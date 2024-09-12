using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using RagChatApi.Models;
using RagChatApi.Models.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public class CosmosArticleService(CosmosContainerClientFactory clientFactory, IEmbeddingService embeddingService) : IArticleService
{
    private readonly Container _client = clientFactory.CreateClient();

    public async Task<Article> CreateArticleFromFileAsync(string category, IFormFile file, CancellationToken cancellationToken = default)
    {
        var embedding = await embeddingService.GetEmbeddingAsync(file, cancellationToken);

        var article = new CosmosArticle
        {
            Id = Guid.NewGuid(),
            Category = category,
            Vector = embedding,
        };

        var partitionKey = new PartitionKeyBuilder()
            .Add(article.Type)
            .Add(article.Category)
            .Build();

        var options = new ItemRequestOptions
        {
            EnableContentResponseOnWrite = false,
        };

        await _client.CreateItemAsync(article, partitionKey, options, cancellationToken: cancellationToken);

        return article;
    }

    public async Task DeleteArticleAsync(Article article, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(CosmosTypes.Article)
            .Add(article.Category)
            .Build();

        var options = new ItemRequestOptions
        {
            EnableContentResponseOnWrite = false,
        };

        await _client.DeleteItemAsync<Article>(article.Id.ToString(), partitionKey, options, cancellationToken: cancellationToken);
    }

    public async IAsyncEnumerable<Article> GetArticlesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var options = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(CosmosTypes.Article),
        };

        var iterator = _client
            .GetItemLinqQueryable<CosmosArticle>(requestOptions: options)
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            var feed = await iterator.ReadNextAsync(cancellationToken);
            foreach (var item in feed)
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<Article> GetTopArticlesByTextAsync(
        string text, 
        string category,
        int top,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var embedding = await embeddingService.GetEmbeddingAsync(text, cancellationToken);

        var query = $"""
            SELECT TOP {top} c.id, VectorDistance(c.vector,@embedding) AS similarity
            FROM c
            ORDER BY VectorDistance(c.vector,@embedding)
            """;

        var queryDefinition = new QueryDefinition(query)
            .WithParameter("@embedding", embedding);

        var partitionKey = new PartitionKeyBuilder()
            .Add(CosmosTypes.Article)
            .Add(category)
            .Build();

        var options = new QueryRequestOptions
        {
            PartitionKey = partitionKey,
        };

        using var iterator = _client.GetItemQueryIterator<CosmosSimilarity>(queryDefinition, requestOptions: options);

        while (iterator.HasMoreResults)
        {
            var feed = await iterator.ReadNextAsync(cancellationToken);
            foreach (var item in feed)
            {
                yield return new Article()
                {
                    Category = category,
                    Id = Guid.Parse(item.Id),
                };
            }
        }
    }
}

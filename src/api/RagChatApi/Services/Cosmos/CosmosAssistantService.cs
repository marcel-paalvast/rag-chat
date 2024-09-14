using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RagChatApi.Models;
using RagChatApi.Models.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services.Cosmos;
public class CosmosAssistantService(CosmosContainerClientFactory clientFactory) : IAssistantService
{
    private readonly Container _client = clientFactory.CreateClient();

    public async Task CreateAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default)
    {
        var cosmosAssistant = new CosmosAssistant()
        {
            Id = assistant.Id,
            Name = assistant.Name,
            Category = assistant.Category,
            Prompt = assistant.Prompt,
        };

        var partitionKey = new PartitionKeyBuilder()
            .Add(cosmosAssistant.Type)
            .Add(cosmosAssistant.Category)
            .Build();

        var options = new ItemRequestOptions
        {
            EnableContentResponseOnWrite = false,
        };

        await _client.CreateItemAsync(cosmosAssistant, partitionKey, options, cancellationToken: cancellationToken);
    }

    public async Task DeleteAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(CosmosTypes.Assistant)
            .Add(assistant.Category)
            .Build();

        var options = new ItemRequestOptions
        {
            EnableContentResponseOnWrite = false,
        };

        await _client.DeleteItemAsync<CosmosAssistant>(assistant.Id.ToString(), partitionKey, options, cancellationToken: cancellationToken);
    }

    public async Task<Assistant?> GetAssistantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var options = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(CosmosTypes.Assistant),
        };

        var iterator = _client
            .GetItemLinqQueryable<CosmosAssistant>(requestOptions: options)
            .Where(x => x.Id == id)
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync(cancellationToken))
            {
                return item;
            }
        }

        return null;
    }

    public async IAsyncEnumerable<Assistant> GetAssistantsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var options = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(CosmosTypes.Assistant),
        };

        var iterator = _client
            .GetItemLinqQueryable<CosmosAssistant>(requestOptions: options)
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
}

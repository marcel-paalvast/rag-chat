using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RagChatApi.Settings;
using OpenAI.Embeddings;

namespace RagChatApi.Services.OpenAi;
public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;

    public OpenAiEmbeddingService(IOptions<OpenAiSettings> options)
    {
        AzureOpenAIClient openAIClient = new(new Uri(options.Value.Uri), new AzureKeyCredential(options.Value.Key));

        _embeddingClient = openAIClient.GetEmbeddingClient(options.Value.Embedding);
    }

    public async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        EmbeddingGenerationOptions options = new()
        {
            Dimensions = 1024,
        };

        var response = await _embeddingClient.GenerateEmbeddingAsync(text, options, cancellationToken);
        return response.Value.Vector;
    }
}

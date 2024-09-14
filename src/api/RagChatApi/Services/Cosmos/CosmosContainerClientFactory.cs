using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using RagChatApi.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RagChatApi.Services.Cosmos;
public class CosmosContainerClientFactory
{
    private readonly Container container;

    public CosmosContainerClientFactory(IOptions<CosmosSettings> settings)
    {
        var options = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Direct,
            ConsistencyLevel = ConsistencyLevel.Eventual,
            UseSystemTextJsonSerializerWithOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            }
        };

        var client = new CosmosClient(settings.Value.Uri, settings.Value.Key, options);
        var database = client.GetDatabase("ragChat");

        container = CreateContainerIfNotExistsAsync(database, "main").Result;
    }

    private static async Task<Container> CreateContainerIfNotExistsAsync(Database database, string containerId)
    {
        List<Embedding> embeddings =
        [
            new()
            {
                Path = "/vector",
                DataType = VectorDataType.Float32,
                DistanceFunction = DistanceFunction.Cosine,
                Dimensions = 1024,
            },
        ];

        Collection<Embedding> collection = new(embeddings);

        ContainerProperties properties = new(id: containerId, partitionKeyPaths: ["/type", "/category"])
        {
            VectorEmbeddingPolicy = new(collection),
            IndexingPolicy = new IndexingPolicy()
            {
                VectorIndexes =
                [
                    new VectorIndexPath()
                    {
                        Path = "/vector",
                        Type = VectorIndexType.DiskANN,
                    },
                ]
            },
        };
        properties.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
        properties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/vector/*" });

        var containerResponse = await database.CreateContainerIfNotExistsAsync(properties, ThroughputProperties.CreateManualThroughput(400));
        return containerResponse.Container;
    }

    public Container CreateClient() => container;
}

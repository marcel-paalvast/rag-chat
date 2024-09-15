using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using RagChatApi.Settings;
using System;
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
        container = database.GetContainer("main");
    }

    public Container CreateClient() => container;
}

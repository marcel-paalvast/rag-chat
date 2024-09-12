using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RagChatApi.Models.Cosmos;
public class CosmosAssistant : Assistant
{
    [JsonPropertyName("type")]
    public string Type { get; } = CosmosTypes.Assistant;


}

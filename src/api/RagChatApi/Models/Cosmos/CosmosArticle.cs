using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RagChatApi.Models.Cosmos;
public class CosmosArticle : Article
{
    [JsonPropertyName("vector")]
    public required ReadOnlyMemory<float> Vector { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; } = CosmosTypes.Article;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RagChatApi.Models;
public class Article
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }
    [JsonPropertyName("category")]
    public required string Category { get; set; }
}

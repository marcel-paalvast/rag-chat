using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RagChatApi.Models;
public class Assistant
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    [StringLength(128)]
    [JsonPropertyName("category")]
    public required string Category { get; set; }
    [StringLength(32)]
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [Required]
    [StringLength(2048)]
    [JsonPropertyName("prompt")]
    public required string Prompt { get; set; }
}

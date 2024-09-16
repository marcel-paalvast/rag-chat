using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Models.Cosmos;
public class CosmosSimilarity
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; set; }
    [JsonProperty(PropertyName = "similarity")]
    public required double Similarity { get; set; }
}

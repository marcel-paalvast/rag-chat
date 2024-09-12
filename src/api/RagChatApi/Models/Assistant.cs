﻿using Newtonsoft.Json;
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
    [Required]
    [StringLength(2048)]
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Settings;
public record CosmosSettings
{
    public required string Uri { get; init; }
    public required string Key { get; init; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Settings;
public record OpenAiSettings
{
    public required string Uri { get; set; }
    public required string Key { get; set; }
    /// <summary>
    /// The name of the deployment to use for the embeddings.
    /// </summary>
    public required string Embedding { get; set; }
    /// <summary>
    /// The id of the assistant to use for the chat.
    /// </summary>
    public required string Assistant { get; set; }
}

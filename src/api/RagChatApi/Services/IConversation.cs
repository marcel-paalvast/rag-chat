using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IConversation
{
    string Id { get; }
    string Category { get; }
    string ContinuationToken { get => $"{Category}/{Id}"; }
    IAsyncEnumerable<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);
}

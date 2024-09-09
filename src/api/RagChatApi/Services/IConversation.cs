using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IConversation
{
    string Id { get; set; }
    IAsyncEnumerable<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public class OpenAiConversation : IConversation
{
    public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IAsyncEnumerable<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

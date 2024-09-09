using System.Runtime.CompilerServices;

namespace RagChatApi.Services;

public class FakeConversation : IConversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public async IAsyncEnumerable<string> SendMessageAsync(string message, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return await Task.FromResult("Response to: " + message);
    }
}

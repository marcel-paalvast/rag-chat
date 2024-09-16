using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IChatService
{
    Task<IConversation> GetConversationAsync(string continuationToken, CancellationToken cancellationToken = default);
    Task<IConversation> CreateConversationAsync(Guid AssistantId, CancellationToken cancellationToken = default);
}

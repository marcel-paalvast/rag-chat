using RagChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IAssistantService
{
    Task CreateAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default);
    Task DeleteAssistantAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Assistant> GetAssistantAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assistant>> GetAssistantsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Assistant>> GetAssistantsAsync(string category, CancellationToken cancellationToken = default);
    Task<IConversation> GetConversationAsync(string id, Guid AssistantId, CancellationToken cancellationToken = default);
    Task<IConversation> CreateConversationAsync(Guid AssistantId, CancellationToken cancellationToken = default);
}

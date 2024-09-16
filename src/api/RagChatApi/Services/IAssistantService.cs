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
    Task DeleteAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Assistant> GetAssistantsAsync(CancellationToken cancellationToken = default);
    Task<Assistant?> GetAssistantAsync(Guid id, CancellationToken cancellationToken = default);
}

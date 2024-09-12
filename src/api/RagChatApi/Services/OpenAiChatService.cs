using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public class OpenAiChatService(IArticleService articleService, IAssistantService assistantService) : IChatService
{
    public async Task<IConversation> CreateConversationAsync(Guid AssistantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IConversation> GetConversationAsync(string id, Guid AssistantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

using RagChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
internal class FakeAssistantService : IAssistantService
{
    public Task CreateAssistantAsync(Assistant assistant, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<IConversation> CreateConversationAsync(Guid AssistantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IConversation>(new FakeConversation());
    }

    public Task DeleteAssistantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<Assistant> GetAssistantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Assistant { Id = Guid.NewGuid(), Category = "Test Category", Prompt = "Test Prompt" });
    }

    public Task<IEnumerable<Assistant>> GetAssistantsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Assistant>>(
        [
            new() { Id = Guid.NewGuid(), Category = "Test Category", Prompt = "Test Prompt" }
        ]);
    }

    public Task<IEnumerable<Assistant>> GetAssistantsAsync(string category, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Assistant>>(
        [
            new() { Id = Guid.NewGuid(), Category = category, Prompt = "Test Prompt" }
        ]);
    }

    public Task<IConversation> GetConversationAsync(string id, Guid AssistantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IConversation>(new FakeConversation());
    }
}
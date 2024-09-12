using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Options;
using RagChatApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Assistants;
using Microsoft.Extensions.Logging;

namespace RagChatApi.Services.OpenAi;
#pragma warning disable OPENAI001 // AssistantClient is for evaluation purposes only and is subject to change or removal in future updates.
public class OpenAiChatService : IChatService
{
    private readonly AssistantClient _assistantClient;
    private readonly Assistant _assistant;
    private readonly IAssistantService _assistantService;
    private readonly IArticleService _articleService;
    private readonly IStorageService _storageService;
    private readonly ILogger<OpenAiChatService> _logger;

    public OpenAiChatService(
        IOptions<OpenAiSettings> options,
        IArticleService articleService,
        IAssistantService assistantService,
        IStorageService storageService,
        ILogger<OpenAiChatService> logger)
    {
        AzureOpenAIClient openAIClient = new(new Uri(options.Value.Uri), new AzureKeyCredential(options.Value.Key));

        _assistantClient = openAIClient.GetAssistantClient();
        _assistant = _assistantClient.GetAssistant(options.Value.Assistant);

        _assistantService = assistantService;
        _articleService = articleService;
        _storageService = storageService;

        _logger = logger;
    }

    public async Task<IConversation> CreateConversationAsync(Guid AssistantId, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantService.GetAssistantAsync(AssistantId, cancellationToken)
            ?? throw new ArgumentException("Assistant not found", nameof(AssistantId));

        var options = new ThreadCreationOptions()
        {
            InitialMessages = { new ThreadInitializationMessage(MessageRole.Assistant, [MessageContent.FromText(assistant.Prompt)]) },
        };

        var thread = await _assistantClient.CreateThreadAsync(options, cancellationToken);
        return new OpenAiConversation(assistant.Category, _assistant, _assistantClient, thread, _articleService, _storageService, _logger);
    }

    public async Task<IConversation> GetConversationAsync(string continuationToken, CancellationToken cancellationToken = default)
    {
        if (continuationToken.Split('/') is not [var category, var id])
        {
            throw new ArgumentException("Invalid continuation token", nameof(continuationToken));
        }

        var thread = await _assistantClient.GetThreadAsync(id, cancellationToken);
        return new OpenAiConversation(category, _assistant, _assistantClient, thread, _articleService, _storageService, _logger);
    }
}

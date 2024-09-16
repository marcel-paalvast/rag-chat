using Microsoft.Extensions.Logging;
using OpenAI.Assistants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services.OpenAi;
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class OpenAiConversation(string category, Assistant assistant, AssistantClient assistantClient, AssistantThread assistantThread, IArticleService articleService, IStorageService storageService, ILogger logger) : IConversation
{
    public string Id { get; } = assistantThread.Id;
    public string Category { get; } = category;

    public async IAsyncEnumerable<string> SendMessageAsync(string message, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var options = new RunCreationOptions()
        {
            AdditionalMessages = 
            {
                new(MessageRole.User, [MessageContent.FromText(message)]),
                new(MessageRole.Assistant, [MessageContent.FromText("I can use the following snippets of knowledge to answer the user question:")]),
            },
            MaxCompletionTokens = 1000,
            ResponseFormat = AssistantResponseFormat.Text,
            Temperature = 0.4f,
            TruncationStrategy = RunTruncationStrategy.CreateLastMessagesStrategy(30),
        };

        var articles = articleService.GetTopArticlesByTextAsync(message, Category, top: 3, cancellationToken);

        List<Task<string>> tasks = [];
        await foreach (var article in articles)
        {
            tasks.Add(storageService.GetFileAsync(article.Id.ToString(), cancellationToken));
        }

        await Task.WhenAll(tasks);
        options.AdditionalMessages.Add(new(MessageRole.Assistant, tasks.Select(x => MessageContent.FromText(x.Result)).ToList()));

        var streamingUpdates = assistantClient.CreateRunStreamingAsync(assistantThread, assistant, options);

        await foreach (var streamingUpdate in streamingUpdates.WithCancellation(cancellationToken))
        {
            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCompleted && streamingUpdate is RunUpdate runUpdate)
            {
                var usage = runUpdate.Value.Usage;
                logger.LogTrace(
                    "Token used: prompt {prompt}; completion {completion}; total {total}",
                    usage.PromptTokens,
                    usage.CompletionTokens,
                    usage.TotalTokens);
            }

            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                if (contentUpdate.Text is not null)
                {
                    yield return contentUpdate.Text;
                }
            }
        }
    }
}

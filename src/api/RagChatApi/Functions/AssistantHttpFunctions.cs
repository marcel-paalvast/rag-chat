using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RagChatApi.Models;
using RagChatApi.Services;
using System.ComponentModel.DataAnnotations;

namespace RagChatApi.Functions;

public class AssistantHttpFunctions(
    IAssistantService assistantService,
    IChatService chatService,
    ILogger<ArticleHttpFunctions> logger)
{
    const string continuationTokenHeader = "Continuation-Token";

    [Function("PostAssistants")]
    public async Task<IActionResult> PostAssistants(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "assistants")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var assistant = await req.ReadFromJsonAsync<Assistant>(cancellationToken);

        if (assistant == null)
        {
            return new BadRequestObjectResult("Assistant is required");
        }

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(assistant, new ValidationContext(assistant), validationResults, true))
        {
            return new BadRequestObjectResult(string.Join(", ", validationResults.Select(v => v.ErrorMessage)));
        }

        assistant.Id = Guid.NewGuid();
        await assistantService.CreateAssistantAsync(assistant, cancellationToken);
        logger.LogInformation("C# HTTP trigger function processed a POST request for assistants.");
        return new OkObjectResult(assistant);
    }

    [Function("GetAssistantChat")]
    public async Task<IActionResult> GetAssistantChat(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants/{assistant}/chat")] HttpRequest req,
        string assistant,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(assistant, out Guid assistantId))
        {
            return new BadRequestObjectResult("Invalid assistant id");
        }

        if (!req.Query.TryGetValue("message", out StringValues messageValues) || messageValues.FirstOrDefault() is not string message)
        {
            return new BadRequestObjectResult("Message is required");
        }

        if (message.Length < 1 || message.Length > 2048)
        {
            return new BadRequestObjectResult("Message must be between 1 and 2048 characters");
        }

        IConversation conversation = req.Headers.TryGetValue("continuationToken", out StringValues headerValues) && headerValues.FirstOrDefault() is string conversationId
            ? await chatService.GetConversationAsync(conversationId, assistantId, cancellationToken)
            : await chatService.CreateConversationAsync(assistantId, cancellationToken);

        var messageStream = conversation.SendMessageAsync(message, cancellationToken);

        req.HttpContext.Response.Headers.Append(continuationTokenHeader, conversation.Id);
        return new ChatStreamResult(messageStream);
    }

    [Function("GetAssistants")]
    public async Task<IActionResult> GetAssistants(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var assistants = await assistantService.GetAssistantsAsync(cancellationToken).ToListAsync(cancellationToken: cancellationToken);

        logger.LogInformation("C# HTTP trigger function processed a GET request for assistants.");
        return new OkObjectResult(assistants);
    }

    [Function("DeleteAssistant")]
    public async Task<IActionResult> DeleteAssistant(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "assistants")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var assistant = await req.ReadFromJsonAsync<Assistant>(cancellationToken);

        if (assistant == null)
        {
            return new BadRequestObjectResult("Assistant is required");
        }

        await assistantService.DeleteAssistantAsync(assistant, cancellationToken);
        logger.LogInformation($"C# HTTP trigger function processed a DELETE request for assistant: {assistant}");
        return new OkObjectResult($"Assistant {assistant} deleted");
    }
}

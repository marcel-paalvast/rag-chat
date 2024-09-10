using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RagChatApi.Models;
using RagChatApi.Services;

namespace RagChatApi.Functions
{
    public class HttpFunctions(IArticleService articleService, IAssistantService assistantService, ILogger<HttpFunctions> logger)
    {
        const string continuationTokenHeader = "Continuation-Token";

        [Function("GetArticles")]
        public async Task<IActionResult> GetArticles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "articles")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            var articles = await articleService.GetArticlesAsync(cancellationToken);
            logger.LogInformation("C# HTTP trigger function processed a GET request for articles.");
            return new OkObjectResult(articles);
        }

        [Function("PostArticles")]
        public async Task<IActionResult> PostArticles(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "articles")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult("Invalid request");
            }

            if (req.Form.Files.Count != 1)
            {
                return new BadRequestObjectResult("Only a single file is supported");
            }

            if (req.Form["category"].FirstOrDefault() is not string category)
            {
                return new BadRequestObjectResult("Category is required");
            }

            var article = new Article
            {
                Id = Guid.NewGuid(),
                Category = category,
                //something with embeddings?
            };
            await articleService.CreateArticleAsync(article, cancellationToken);
            logger.LogInformation("C# HTTP trigger function processed a POST request for articles.");
            return new OkObjectResult(article);
        }

        [Function("DeleteArticle")]
        public async Task<IActionResult> DeleteArticle(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "articles/{article}")] HttpRequest req,
            string article,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(article, out Guid articleId))
            {
                return new BadRequestObjectResult("Invalid article id");
            }

            await articleService.DeleteArticleAsync(articleId, cancellationToken);
            logger.LogInformation($"C# HTTP trigger function processed a DELETE request for article: {article}");
            return new OkObjectResult($"Article {article} deleted");
        }

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

            IConversation conversation = req.Headers.TryGetValue("continuationToken", out StringValues headerValues) && headerValues.FirstOrDefault() is string conversationId
                ? await assistantService.GetConversationAsync(conversationId, assistantId, cancellationToken)
                : await assistantService.CreateConversationAsync(assistantId, cancellationToken);

            var messageStream = conversation.SendMessageAsync(message, cancellationToken);

            req.HttpContext.Response.Headers.Append(continuationTokenHeader, conversation.Id);
            return new ChatStreamResult(messageStream);
        }

        [Function("GetAssistants")]
        public async Task<IActionResult> GetAssistants(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            IEnumerable<Assistant> assistants = req.Query.TryGetValue("category", out StringValues categoryValues) && categoryValues.FirstOrDefault() is string category
                ? await assistantService.GetAssistantsAsync(category, cancellationToken)
                : await assistantService.GetAssistantsAsync(cancellationToken);

            logger.LogInformation("C# HTTP trigger function processed a GET request for assistants.");
            return new OkObjectResult(assistants);
        }

        [Function("DeleteAssistant")]
        public async Task<IActionResult> DeleteAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "assistants/{assistant}")] HttpRequest req,
            string assistant,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(assistant, out Guid assistantId))
            {
                return new BadRequestObjectResult("Invalid article id");
            }

            await assistantService.DeleteAssistantAsync(assistantId, cancellationToken);
            logger.LogInformation($"C# HTTP trigger function processed a DELETE request for assistant: {assistant}");
            return new OkObjectResult($"Assistant {assistant} deleted");
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RagChatApi.Models;
using RagChatApi.Services;
using System.ComponentModel.DataAnnotations;

namespace RagChatApi.Functions;

public class ArticleHttpFunctions(
    IArticleService articleService, 
    ILogger<ArticleHttpFunctions> logger)
{
    [Function("GetArticles")]
    public async Task<IActionResult> GetArticles(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "articles")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var articles = await articleService.GetArticlesAsync(cancellationToken).ToListAsync(cancellationToken);
        
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

        if (!req.Form.TryGetValue("category", out var categoryValues) || categoryValues.FirstOrDefault() is not string category)
        {
            return new BadRequestObjectResult("Category is required");
        }

        if (category.Length < 1 || category.Length > 128)
        {
            return new BadRequestObjectResult("Category must be between 1 and 128 characters");
        }

        var file = req.Form.Files.Single();
        var article = await articleService.CreateArticleFromFileAsync(category, file, cancellationToken);

        logger.LogInformation("C# HTTP trigger function processed a POST request for articles.");
        return new OkObjectResult(article);
    }

    [Function("DeleteArticle")]
    public async Task<IActionResult> DeleteArticle(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "article")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var article = await req.ReadFromJsonAsync<Article>(cancellationToken);

        if (article == null)
        {
            return new BadRequestObjectResult("Article is required");
        }

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(article, new ValidationContext(article), validationResults, true))
        {
            return new BadRequestObjectResult(string.Join(", ", validationResults.Select(v => v.ErrorMessage)));
        }

        await articleService.DeleteArticleAsync(article, cancellationToken);
        logger.LogInformation($"C# HTTP trigger function processed a DELETE request for article: {article}");
        return new OkObjectResult($"Article {article} deleted");
    }
}
using RagChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IArticleService
{
    Task<Article> CreateArticleAsync(string category, string text, CancellationToken cancellationToken = default);
    Task DeleteArticleAsync(Article article, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Article> GetArticlesAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<Article> GetTopArticlesByTextAsync(string text, string category, int top, CancellationToken cancellationToken = default);
}

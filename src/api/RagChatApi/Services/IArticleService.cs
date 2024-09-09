using RagChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IArticleService
{
    Task<IEnumerable<Article>> GetArticlesAsync(CancellationToken cancellationToken = default);
    Task<Article> CreateArticleAsync(Article article, CancellationToken cancellationToken = default);
    Task DeleteArticleAsync(Guid id, CancellationToken cancellationToken = default);
}

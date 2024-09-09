using RagChatApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
internal class FakeArticleService : IArticleService
{
    public Task<Article> CreateArticleAsync(Article article, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(article);
    }

    public Task DeleteArticleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Article>> GetArticlesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Article>>(
        [
            new Article { Id = Guid.NewGuid(), Category = "Test Category" },
        ]);
    }
}

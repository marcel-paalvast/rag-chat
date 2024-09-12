using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
internal class OpenAiEmbeddingService : IEmbeddingService
{
    public Task<double[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<double[]> GetEmbeddingAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

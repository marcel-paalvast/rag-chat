using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IEmbeddingService
{
    Task<double[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<double[]> GetEmbeddingAsync(IFormFile file, CancellationToken cancellationToken = default);
}

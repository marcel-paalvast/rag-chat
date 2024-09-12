using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public interface IStorageService
{
    Task<string> GetFileAsync(string id, CancellationToken cancellationToken = default);
    Task SaveFileAsync(string id, string data, CancellationToken cancellationToken = default);
}

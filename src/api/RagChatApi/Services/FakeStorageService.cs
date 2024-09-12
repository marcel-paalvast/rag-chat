using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services;
internal class FakeStorageService : IStorageService
{
    public Task<string> GetFileAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("""
            - Milk
            - Eggs
            - Bread
            - Butter
            - Cheese
            - Apples
            - Bananas
            - Chicken
            - Rice
            - Pasta
            """);
    }

    public Task SaveFileAsync(string id, string data, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

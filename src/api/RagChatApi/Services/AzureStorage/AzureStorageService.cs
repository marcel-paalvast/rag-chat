using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using RagChatApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Services.AzureStorage;
internal class AzureStorageService : IStorageService
{
    private readonly BlobContainerClient _container;

    public AzureStorageService(IOptions<AzureStorageSettings> options)
    {
        var client = new BlobServiceClient(options.Value.ConnectionString);
        _container = client.GetBlobContainerClient("rag-chat");
    }

    public async Task<string> GetFileAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _container
            .GetBlobClient(id)
            .DownloadStreamingAsync(cancellationToken: cancellationToken);
        using var reader = new StreamReader(response.Value.Content, Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    public async Task SaveFileAsync(string id, string data, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var response = await _container
            .GetBlobClient(id)
            .UploadAsync(stream, cancellationToken: cancellationToken);
    }
}

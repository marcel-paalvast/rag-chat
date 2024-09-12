using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RagChatApi.Services;
public static class TextExtractor
{
    public static async Task<string> ExtractTextAsync(Stream stream, string extension, CancellationToken cancellationToken = default)
    {
        return extension switch
        {
            "txt" => await GetTextFromPlainTextFileAsync(stream, cancellationToken),
            "md" => await GetTextFromPlainTextFileAsync(stream, cancellationToken),
            _ => throw new NotSupportedException($"File type {extension} is not supported."),
        };
    }

    private static async Task<string> GetTextFromPlainTextFileAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}

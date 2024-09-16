using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi.Models;
/// <summary>
/// Produces a streamed text using a <see cref="StatusCodes.Status200OK"/> response.
/// </summary>
public class ChatStreamResult(IAsyncEnumerable<string> messageStream) : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = "text/plain";

        await foreach (var item in messageStream)
        {
            await response.WriteAsync(item);
            await response.Body.FlushAsync();
        }
    }
}

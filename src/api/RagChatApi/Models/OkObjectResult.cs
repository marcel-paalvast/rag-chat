using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RagChatApi.Models;
/// <summary>
/// An <see cref="ObjectResult"/> that when executed performs content negotiation, formats the entity body, and
/// will produce a <see cref="StatusCodes.Status200OK"/> response if negotiation and formatting succeed.
/// Will maintain the object type for serialization.
/// </summary>
public class OkObjectResult<T> : ContentResult
{
    /// <summary>
    /// Creates an instance of <see cref="OkObjectResult{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    public OkObjectResult(T value) : base()
    {
        Content = JsonSerializer.Serialize(value);
        StatusCode = 200;
        ContentType = "application/json";
    }
}

using Contracts.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shared.ApiResponse;
using System.Data.Common;
using System.Net;
using System.Text.Json;

namespace TaskManagementSystem.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILoggerManager _loggerManager;

    public GlobalExceptionHandler(ILoggerManager loggerManager)
    {
        _loggerManager = loggerManager;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        _loggerManager.LogCritical($"An Error Occurred: {JsonSerializer.Serialize(exception)}");

        IExceptionHandlerFeature? contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

        ProblemDetails errorResponse = new ProblemDetails();

        errorResponse.Status = contextFeature.Error switch
        {
            DbException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        errorResponse.Title = contextFeature.Error.Message;
        errorResponse.Detail = contextFeature?.Error?.InnerException?.Message;
        errorResponse.Type = contextFeature?.Error?.GetType().ToString();

        _loggerManager.LogCritical($"Stack Trace: {contextFeature?.Error?.StackTrace.ToString()}");

        var response = GenericResponse<object?>.Failure(null, HttpStatusCode.InternalServerError, "An Error Occurred.", errorResponse);

        await httpContext.Response.WriteAsJsonAsync(response.ToString());

        return true;
    }
}

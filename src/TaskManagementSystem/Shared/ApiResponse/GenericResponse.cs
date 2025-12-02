using System.Net;
using System.Text.Json;

namespace Shared.ApiResponse;

public class GenericResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; }
    public object? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public GenericResponse(T? data, HttpStatusCode httpStatusCode, bool isSuccess, string message, object? error = null)
    {
        Data = data;
        Message = message;
        IsSuccessful = isSuccess;
        Error = error;
        StatusCode = httpStatusCode;
    }

    public static GenericResponse<T> Success(T? data, HttpStatusCode httpStatusCode, string message) => new GenericResponse<T>(data, httpStatusCode, true, message);
    public static GenericResponse<T> Failure(T? data, HttpStatusCode httpStatusCode, string message, object error) => new GenericResponse<T>(data, httpStatusCode, false, message, error);

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

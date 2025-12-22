using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.UserTask;

public class CancelUserTaskHandler
{
    private HttpClient _httpClient;
    public CancelUserTaskHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(CancelUserTaskDto cancelUserTask)
    {
        try
        {
            var htppResponse = await _httpClient.PostAsJsonAsync("api/TaskUsers/cancel-user-task", cancelUserTask);

            string httpResponseContent = await htppResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if (responseBody is null)
            {
                return (false, "Operationn Failed");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, "An Error Occurred.");
        }
    }
}

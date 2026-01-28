using Shared.ApiResponse;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.UserTask;

public class MarkUserTaskAsCompleteHandler
{
    private readonly HttpClient _httpClient;

    public MarkUserTaskAsCompleteHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(string responseMessage, bool isSuccessful)> Handle(Shared.DataTransferObjects.TaskUser.UpdateUserTaskCompleteStatusDto updateUserTaskCompleteStatus)
    {
        try
        {
            var httpResponse = await _httpClient.PutAsJsonAsync("api/TaskUsers/update-user-task-status", updateUserTaskCompleteStatus);

            var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if(responseBody is null)
            {
                return ("Operation Failed", false);    
            }

            return (responseBody.Message, responseBody.IsSuccessful);
        }
        catch (Exception ex)
        {
            return ("An Error Occurred.", false);
        }
    }
}

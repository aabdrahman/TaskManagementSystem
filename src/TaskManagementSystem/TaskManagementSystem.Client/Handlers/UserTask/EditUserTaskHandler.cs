using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.UserTask;

public class EditUserTaskHandler
{
    private HttpClient _httpClient;
    public EditUserTaskHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(UpdateTaskUserDto updateTaskUserD)
    {
        try
        {
            var httpResponseMessage = await _httpClient.PutAsJsonAsync("api/TaskUsers/update-user-task", updateTaskUserD);

            string httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            GenericResponse<TaskUserDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<TaskUserDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "Operation Failed.");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}

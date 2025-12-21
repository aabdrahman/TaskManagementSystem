using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.UserTask;

public class GetUserTasksHandler
{
    private HttpClient _httpClient;

    public GetUserTasksHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(string responseMessage, IEnumerable<TaskUserDto> userTasks)> Handle(int UserId)
    {
        try
        {
            var getUserTasksHandlerResponse = await _httpClient.GetAsync($"api/TaskUsers/userid/{UserId}?hasQueryFilter=true");

            string responseContent = await getUserTasksHandlerResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<TaskUserDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<TaskUserDto>>>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return ("An Error Occured.", []);
            }

            return (responseBody.Message, responseBody?.Data ?? []);
        }
        catch (Exception ex)
        {
            return (responseMessage: $"Fetch User Tasks Failed.", []);
        }
    }
}

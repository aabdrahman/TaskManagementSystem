using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.CreatedTask;

public class FetchCreatedTaskAssignedTasksHandler
{
    private HttpClient _httpClient;

    public FetchCreatedTaskAssignedTasksHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<GenericResponse<IEnumerable<TaskUserDto>>> Handle(int TaskId)
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync($"api/TaskUsers/taskid/{TaskId}?hasQueryFilter=false");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<TaskUserDto>> responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<TaskUserDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return null;
            }

            return responseBody;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.UserTask;

public class AddNewUserTaskHandler
{
    private HttpClient _httpClient;
    public AddNewUserTaskHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(CreateTaskUserDto createTaskUser)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/TaskUsers", createTaskUser);
            
            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<TaskUserDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<TaskUserDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody == null)
            {
                return (false, "Operation Failed.");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, "An Error Occurred.");
        }
    }
}

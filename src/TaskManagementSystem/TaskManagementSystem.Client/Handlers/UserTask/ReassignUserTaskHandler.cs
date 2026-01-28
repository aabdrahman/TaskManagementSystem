using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.UserTask;

public class ReassignUserTaskHandler
{
    private HttpClient _httpClient;
    public ReassignUserTaskHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(ReassignTaskUserDto reassignTaskUserDetails)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/TaskUsers/reassign-user-task", reassignTaskUserDetails);

            var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "Operation Failed");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
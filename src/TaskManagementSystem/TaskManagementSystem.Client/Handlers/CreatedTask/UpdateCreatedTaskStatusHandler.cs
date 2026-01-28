using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.CreatedTask;

public class UpdateCreatedTaskStatusHandler
{
    private HttpClient _httpClient;
    public UpdateCreatedTaskStatusHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(UpdateCreatedTaskStatusDto updateCreatedTaskStatus)
    {
        try
        {
            var httpResponse = await _httpClient.PutAsJsonAsync("api/CreatedTasks/update-task-status", updateCreatedTaskStatus);

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();    

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "An Error Occurred");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}

using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.CreatedTask;

public class EditCreatedTaskHandler
{
    private HttpClient _httpClient;
    public EditCreatedTaskHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(UpdateCreatedTaskDto updateCreatedTask)
    {
        try
        {
            var httpResponse = await _httpClient.PutAsJsonAsync("api/CreatedTasks/update-task", updateCreatedTask);
            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<CreatedTaskDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<CreatedTaskDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "Operation Failed");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, $"An Error Occurred: {ex.Message}");
        }
    }
}

using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.CreatedTask;

public class FetchUserCreatedTasksHandler
{
    private readonly HttpClient _httpClient;

    public FetchUserCreatedTasksHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool _isSuccessful, IEnumerable<CreatedTaskDto> userCreatedTasks)> Handle(int UserId)
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync($"api/CreatedTasks/userid/{UserId}?hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<CreatedTaskDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<CreatedTaskDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if (responseBody is null)
            {
                return (false, []);
            }

            return (responseBody.IsSuccessful, responseBody.Data ?? []);
        }
        catch (Exception ex)
        {
            return (false, []);
        }
    }
}

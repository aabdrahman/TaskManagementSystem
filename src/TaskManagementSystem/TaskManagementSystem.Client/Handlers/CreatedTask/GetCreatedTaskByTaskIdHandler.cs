using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.CreatedTask;

public class GetCreatedTaskByTaskIdHandler
{
    private readonly HttpClient _httpClient;

    public GetCreatedTaskByTaskIdHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<GenericResponse<CreatedTaskDto>> Handle(string TaskId)
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync($"api/CreatedTasks/taskid/{TaskId}?hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<CreatedTaskDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<CreatedTaskDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

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

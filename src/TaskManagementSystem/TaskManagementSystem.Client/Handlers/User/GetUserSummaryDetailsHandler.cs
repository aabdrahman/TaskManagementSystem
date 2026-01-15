using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.User;

public class GetUserSummaryDetailsHandler
{
    private HttpClient _httpClient;
    public GetUserSummaryDetailsHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<IEnumerable<UserSummaryDto>> Handle()
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync("api/Users?trackChanges=false&hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<UserSummaryDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<UserSummaryDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody == null)
            {
                return [];
            }

            return responseBody.IsSuccessful ? responseBody.Data : [];
        }
        catch (Exception ex)
        {
            return [];
        }
    }
}

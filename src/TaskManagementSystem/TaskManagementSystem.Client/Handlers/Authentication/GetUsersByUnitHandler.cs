using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class GetUsersByUnitHandler
{
    private HttpClient _httpClient;
    public GetUsersByUnitHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<IEnumerable<UserDto>> Handle(int UnitId)
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync("api/Users/getByUnitId/1?hasQueryFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<UserDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<UserDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody == null )
            {
                return [];
            }

            return responseBody.Data ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }
}

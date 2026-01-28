using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class GetUnitMemberHandler
{
    private HttpClient _httpClient;

    public GetUnitMemberHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<GenericResponse<IEnumerable<UserSummaryDto>>> Handle(int UserId)
    {
        try
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/Users/getUnitMember/{UserId}");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<UserSummaryDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<UserSummaryDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody == null)
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

using Shared.ApiResponse;
using Shared.DataTransferObjects.Role;
using System.Net.Http;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Role;

public class GetRolesHandler
{
    private readonly HttpClient _httpClient;

    public GetRolesHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);    
    }

    public async Task<IEnumerable<RoleDto>> Handle()
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync("api/Roles?hasQueyFilter=true");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<IEnumerable<RoleDto>>? responseBody = JsonSerializer.Deserialize<GenericResponse<IEnumerable<RoleDto>>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return [];
            }

            return responseBody.Data ?? [];
        }
        catch (Exception ex)
        {
            return [];
        }
    }
}

using Shared.ApiResponse;
using Shared.DataTransferObjects.Role;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Role;

public class AddRoleHandler
{
    private readonly HttpClient _httpClient;
    public AddRoleHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(CreateRoleDto createRole)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/Roles", createRole);

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<RoleDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<RoleDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "An Error Occurred.");
            }

            return (responseBody.IsSuccessful, responseBody.Message);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}

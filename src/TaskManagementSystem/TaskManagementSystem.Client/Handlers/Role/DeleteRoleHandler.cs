using Shared.ApiResponse;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Role;

public class DeleteRoleHandler
{
    private HttpClient _httpClient;

    public DeleteRoleHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(int RoleId)
    {
        try
        {
            var httpResponse = await _httpClient.DeleteAsync($"api/Roles/{RoleId}?isSoftDelete=false");

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

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
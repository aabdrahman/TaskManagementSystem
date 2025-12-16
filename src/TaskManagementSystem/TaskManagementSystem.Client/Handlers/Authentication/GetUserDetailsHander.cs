using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class GetUserDetailsHander
{
    private HttpClient _httpClient;

    public GetUserDetailsHander(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<UserDto?> Handle(int UserId)
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync($"api/Users/{UserId}?hasQueryFilter=true");

            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<UserDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<UserDto>>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return null;
            }

            if(!responseBody.IsSuccessful)
            {
                return null;
            }

            return responseBody.Data;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}

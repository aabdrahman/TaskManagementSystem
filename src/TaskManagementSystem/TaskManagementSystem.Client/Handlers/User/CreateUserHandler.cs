using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.User;

public class CreateUserHandler
{
    private readonly HttpClient _httpClient;

    public CreateUserHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
    }

    public async Task<(bool isSuccessful, string responseMessage)> Handle(CreateUserDto createUser)
    {
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/Users", createUser);

            string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

            GenericResponse<UserDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<UserDto>>(httpResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

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

using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class ChangePasswordHandler
{
	private HttpClient _httpClient;

    public ChangePasswordHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ClientHelper.OpenClientKey);
    }

    public async Task<(bool isSuccess, string message)> Handle(ChangeUserPasswordDto changeUserPasswordDto)
    {
		try
		{
			var httpResponse = await _httpClient.PostAsJsonAsync("api/users/change-password", changeUserPasswordDto);

			var responseContent = await httpResponse.Content.ReadAsStringAsync();

			GenericResponse<string>? responseBody = JsonSerializer.Deserialize<GenericResponse<string>>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

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

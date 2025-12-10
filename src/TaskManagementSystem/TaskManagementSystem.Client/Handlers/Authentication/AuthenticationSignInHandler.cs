using Blazored.LocalStorage;
using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class AuthenticationSignInHandler
{
	private ILocalStorageService _localStorageService;
	private HttpClient _httpClient;
	public AuthenticationSignInHandler(ILocalStorageService localStorageService, IHttpClientFactory httpClientFactory)
	{
		_localStorageService = localStorageService;
		_httpClient = httpClientFactory.CreateClient(ClientHelper.OpenClientKey);
	}

	public async Task<(bool isSuccessful, string message)> Handle(UserToLoginDto userToLoginDto)
	{
		try
		{
			var httpResponse = await _httpClient.PostAsJsonAsync("api/Users/login", userToLoginDto);

			string responseContent = await httpResponse.Content.ReadAsStringAsync();

			GenericResponse<TokenDto>? responseBody = JsonSerializer.Deserialize<GenericResponse<TokenDto>>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

			if(responseBody is null)
			{
				return (false, "An Error Occurred.");
			}

			if(responseBody.IsSuccessful)
			{
				await _localStorageService.SetItemAsync<TokenDto>(ClientHelper.TokenSessionStorgaeKey, responseBody.Data);
			}
			
			return (responseBody.IsSuccessful, responseBody.Message);
		}
		catch (HttpRequestException ex)
		{
			return (false, ex.Message);
		}
	}
}

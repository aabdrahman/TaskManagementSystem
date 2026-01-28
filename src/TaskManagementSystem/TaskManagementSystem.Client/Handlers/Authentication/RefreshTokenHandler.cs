using Blazored.LocalStorage;
using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagementSystem.Client.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class RefreshTokenHandler
{
    private ILocalStorageService _localStorageService;
    private HttpClient _httpClient;

    public RefreshTokenHandler(ILocalStorageService localStorageService, IHttpClientFactory httpClientFactory)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(ClientHelper.OpenClientKey);
    }

    public async Task<(bool, string)> Handle(TokenDto tokenToRefresh)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Users/refresh", tokenToRefresh, new CancellationToken());

            response.EnsureSuccessStatusCode();

            GenericResponse<TokenDto?>? responseBody = JsonSerializer.Deserialize<GenericResponse<TokenDto?>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? null;

            if(responseBody is null)
            {
                return (false, "Invalid Response.");
            }
            else
            {
                if(!responseBody.IsSuccessful)
                {
                    return (responseBody.IsSuccessful, responseBody.Message);
                }

                TokenDto? generatedToken = responseBody.Data;

                if(generatedToken is null)
                {
                    return (responseBody.IsSuccessful, responseBody.Message);
                }

                await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);

                await _localStorageService.SetItemAsync<TokenDto>(ClientHelper.TokenSessionStorgaeKey, generatedToken);

                return (responseBody.IsSuccessful, responseBody.Message);
            }
        }
        catch (Exception ex)
        {
            return (false, "Error Occurred");
        }
    }
}

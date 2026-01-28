using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManagementSystem.Client.Helper;

namespace TaskManagementSystem.Client.Handlers.Authentication;

public class AuthenticationSignoutHandler
{
    private ILocalStorageService _localStorageService;
    private HttpClient _httpClient;
    private AuthenticationStateProvider _stateProvider;
    public AuthenticationSignoutHandler(ILocalStorageService localStorageService, IHttpClientFactory httpClientFactory, AuthenticationStateProvider stateProvider)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClientFactory.CreateClient(ClientHelper.SecureClientKey);
        _stateProvider = stateProvider;
    }

    public async Task<bool> Handle()
    {
        try
        {
            await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);
            await ((AuthStateProvider)_stateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = default;

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

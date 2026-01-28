using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Shared.DataTransferObjects.User;
using TaskManagementSystem.Client.Handlers.Authentication;
using TaskManagementSystem.Client.Helper;
namespace TaskManagementSystem.Client.AuthenticationProvider;

public class AuthStateHandler : DelegatingHandler
{
    private ILocalStorageService _localStorageService;
    private RefreshTokenHandler _refreshTokenHandler;

    public AuthStateHandler(ILocalStorageService localStorageService, RefreshTokenHandler refreshTokenHandler)
    {
        _localStorageService = localStorageService;
        _refreshTokenHandler = refreshTokenHandler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        TokenDto? sessionToken = await _localStorageService.GetItemAsync<TokenDto>(ClientHelper.TokenSessionStorgaeKey, cancellationToken) ?? null;

        if(sessionToken is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(ClientHelper.AuthorizationHeaderKey, sessionToken.Token);
        }

        var result = await base.SendAsync(request, cancellationToken);

        if (result.IsSuccessStatusCode)
        {
            return result;
        }

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            try
            {
                if (sessionToken is not null)
                {
                    var refreshTokenResponse = await _refreshTokenHandler.Handle(sessionToken);

                    if (refreshTokenResponse.Item1)
                    {
                        sessionToken = await _localStorageService.GetItemAsync<TokenDto>(ClientHelper.TokenSessionStorgaeKey, cancellationToken);

                        request.Headers.Authorization = new AuthenticationHeaderValue(ClientHelper.AuthorizationHeaderKey, sessionToken.Token);

                        result = await base.SendAsync(request, cancellationToken);
                    }
                }

                return result;
            }
            catch (HttpRequestException ex)
            {

                return result;
            }
        }


        return result;
    }
}

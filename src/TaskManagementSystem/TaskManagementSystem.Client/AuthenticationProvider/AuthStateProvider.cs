using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.DataTransferObjects.User;
using System.Security.Claims;
using TaskManagementSystem.Client.Handlers.Authentication;

namespace TaskManagementSystem.Client.Helper;

public class AuthStateProvider : AuthenticationStateProvider
{
    private ILocalStorageService _localStorageService;
    private RefreshTokenHandler _refreshTokenHandler;
    private AuthenticationState _anonymous;

    public AuthStateProvider(ILocalStorageService localStorageService, RefreshTokenHandler refreshTokenHandler)
    {
        _localStorageService = localStorageService;
        _refreshTokenHandler = refreshTokenHandler;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //Fetch Token from browser storage
        TokenDto? sessionToken = await _localStorageService.GetItemAsync<TokenDto>(ClientHelper.TokenSessionStorgaeKey, new CancellationToken());


        if(sessionToken is null)
        {
            Console.WriteLine("No token found");
            //Set to anonymnous as token from storage is null.
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
            return _anonymous;
        }

        //Fetch claims principal of the token from storage
        var claimsPrincipal = JwtParser.ParseClaimsFromJwt(sessionToken.Token);

        //Fetch the expiry time with key: 'exp' from the claims
        string expiryTimeValue = claimsPrincipal.FirstOrDefault(x => x.Type == "exp")?.Value ?? "";

        //Try convert the exp value to a datetime
        if(!long.TryParse(expiryTimeValue, out long expiryTime))
        {
            Console.WriteLine($"Expired Time: {expiryTimeValue}");
            //Set to anonymous and remove the invalid session token from storage
            await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
            return _anonymous;
        }

        //Convert the exp value to a valid datetime
        DateTimeOffset expTime = DateTimeOffset.FromUnixTimeSeconds(expiryTime);

        if (DateTimeOffset.UtcNow >= expTime)
        {
			Console.WriteLine($"Expired Time Elapsed: {expTime}");
			//The expiry time has exceeded. Token already expired. Remove expired token and set anonymous
			await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
            return _anonymous;
        }

        //To check if to refresh the token. Check if the expiry time is within the refresh window value
        var timeToExpiry = (expTime - DateTimeOffset.UtcNow).TotalSeconds;

        if (timeToExpiry <= ClientHelper.GetRefreshTokenWindow)
        {
            try
            {
                Console.WriteLine($"Refresh window..");
                //Genrate a new refresh token
                var tokenRefreshResponse = await _refreshTokenHandler.Handle(sessionToken);

                if(!tokenRefreshResponse.Item1)
                {
                    //Regresh token was not successful. Remove the token and set to anonymous
                    await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);
                    NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
                    return _anonymous;
                }

                //Fetch the valid token from storage as its been updated by the refresh token handler
                sessionToken = await _localStorageService.GetItemAsync<TokenDto>(ClientHelper.TokenSessionStorgaeKey);
                if (sessionToken is null)
                {
                    //I its still empty, rreturn annymous (not actually possible but a valid check)
                    NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
                    return _anonymous;
                }

                //Set claims principal from the valid new token
                claimsPrincipal = JwtParser.ParseClaimsFromJwt(sessionToken.Token);

                //This ensure that the identity is created from the new claims principal by setting a new identity
                //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(
                //        new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(sessionToken.Token),
                //        "jwtAuthType", "Username", ClaimTypes.Role)))));
            }
            catch
            {
                //In case an exception happens when refreshing token, we remove the token from the storage and then set identity as anonymous
                await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);
                NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
                return _anonymous;
            }
        }

        //Append the new valid token to the 'SecureCleint' HttpCleint instance.
        //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ClientHelper.AuthorizationHeaderKey, sessionToken.Token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claimsPrincipal, "jwtAuthType", nameType: ClaimTypes.Name, roleType: ClaimTypes.Role)));
    }

    public async Task NotifyUserLogout()
    {
        await _localStorageService.RemoveItemAsync(ClientHelper.TokenSessionStorgaeKey);
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}

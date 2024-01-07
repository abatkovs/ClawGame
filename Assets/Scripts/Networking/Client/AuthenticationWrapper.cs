
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    private static int _rateLimit = 1000;
    
    //TODO: Improve authentication
    public static async Task<AuthState> DoAuthAsync(int maxTries = 5 )
    {
        if(AuthState == AuthState.Authenticated)
            return AuthState;

        if (AuthState == AuthState.Authenticating)
        {
            Debug.Log("Already authenticating");
            await Authenticating();
            return AuthState;
        }
            

        await SignInAnonymouslyAsync(maxTries);

        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;
        int tries = 0;

        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                //Check if authentication was successful
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authException)
            {
                Debug.Log(authException);
                AuthState = AuthState.Error;
            }
            catch(RequestFailedException requestException)
            {
                Debug.Log(requestException);
                AuthState = AuthState.Error;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                AuthState = AuthState.Error;
            }
            
            tries++;
            //delay to avoid hitting rate limit
            await Task.Delay(_rateLimit);
        }

        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogError($"Failed to authenticate after {maxTries} tries");
            AuthState = AuthState.TimeOut;
        }
            
    }
}

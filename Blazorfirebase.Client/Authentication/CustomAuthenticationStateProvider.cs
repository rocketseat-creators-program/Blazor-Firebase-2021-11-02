using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazorfirebase.Client.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;

        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            var token = await _localStorage.GetItemAsync<string>("tokenJWT");
            if (token != null)
            {
                identity = new ClaimsIdentity(ParseJWTClaims(token), "jwt");
            }
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        public async Task MarkUserAsAuthenticated(string jwt)
        {
            var identity = new ClaimsIdentity();
            var token = await _localStorage.GetItemAsync<string>("tokenJWT");
            if (token != null)
            {
                identity = new ClaimsIdentity(ParseJWTClaims(token), "jwt");
            }
            else
            {
                await _localStorage.SetItemAsync("tokenJWT", jwt);
                identity = new ClaimsIdentity(ParseJWTClaims(jwt), "jwt");
            }
            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(new ClaimsPrincipal(identity))
            ));
        }

        public async Task MarkUserAsLoggedOut()
        {
            var identity = new ClaimsIdentity();

            await _localStorage.RemoveItemAsync("tokenJWT");

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(new ClaimsPrincipal(identity))
                ));
        }
        private List<Claim> ParseJWTClaims(string tokenString)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);
            return token.Claims.ToList();
        }
    }
}
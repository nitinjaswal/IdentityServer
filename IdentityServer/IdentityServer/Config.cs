using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using static Duende.IdentityModel.OidcConstants;
using GrantTypes = Duende.IdentityServer.Models.GrantTypes;

namespace IdentityServer;

public static class Config
{
    //ApiResource: what API the token can access. e.g "api1" is our backend API.
    public static IEnumerable<ApiResource> ApiResources =>
    new ApiResource[]
    {
        new ApiResource("api1")//API name
        {
            Scopes = { "api1" } //What scope the API accepts
        }
    };

    //IdentityResource: contains information about the user in the ID token
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(), //UserId
            new IdentityResources.Profile(),//User profile Info (name, email etc)
            new IdentityResource() //Custom claims
            {
                Name = "verification",
                UserClaims = new List<string>
                {
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified
                }
            }
        };

    //APIScope: It controls what part of the API the client is allowed to access.
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: "api1", displayName: "My API") //only "api1" can be accessed
        };

    //Client: the application that wants to log in users
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "client", //Client Name

                // no interactive user, use the clientid/secret for authentication. machine to machine interaction
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".ToSha256())
                },

                // scopes that client has access to
                AllowedScopes = { "api1" },
                AccessTokenType = AccessTokenType.Jwt
            },

             // interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,//Explicit user login through UI 
                RequirePkce = true,  
                // where to redirect to after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "verification"
                }
            }
        };

}

namespace PasswordProviderSvc.Application.Infrastructure
{
    public static class EnvironmentVariables
    {
        public const string JwtTokenIssuer = "JWT_TOKEN_ISSUER";
        public const string JwtTokenAudience = "JWT_TOKEN_AUDIENCE";
        public const string JwtTokenIdentityProviderPublicKey = "JWT_TOKEN_IDENTITY_PROVIDER_PUBLIC_KEY";
    }
}
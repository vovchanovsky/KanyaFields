namespace ApiSvc.Infrastructure.Infrastructure
{
    public static class EnvironmentVariables
    {
        public const string RedisHost = "REDIS_HOST";
        public const string RedisPort = "REDIS_PORT";
        public const string JwtTokenIssuer = "JWT_TOKEN_ISSUER";
        public const string JwtTokenAudience = "JWT_TOKEN_AUDIENCE";
        public const string JwtTokenIdentityProviderPublicKey = "JWT_TOKEN_IDENTITY_PROVIDER_PUBLIC_KEY";
    }
}
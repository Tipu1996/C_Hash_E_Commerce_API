namespace eCommerceAPI.JwtAuthentication
{
    public record JwtOptions(
    string Issuer,
    string Audience,
    string SigningKey,
    int ExpirationSeconds
);
}
﻿namespace AuthenticationApi.Modules.Jwt;

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
}
﻿namespace NetApp.Application.Dtos.Identity;

public class RefreshTokenRequest
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

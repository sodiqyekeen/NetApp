using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetApp.Application.Common;
using NetApp.Application.Dtos.Common;
using NetApp.Application.Dtos.Identity;
using NetApp.Application.Interfaces;
using NetApp.Application.Interfaces.Identity;
using NetApp.Domain.Constants;
using NetApp.Domain.Entities;
using NetApp.Domain.Exceptions;
using NetApp.Domain.Models;
using NetApp.Infrastructure.Identity.Models;
using NetApp.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetApp.Infrastructure.Identity.Services;

internal class IdentityService : IIdentityService
{
    private readonly UserManager<NetAppUser> _userManager;
    private readonly SignInManager<NetAppUser> _signInManager;
    private readonly RoleManager<NetAppRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IRepositoryProvider _repositoryProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<NetAppLocalizer> _localizer;
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(UserManager<NetAppUser> userManager,
        SignInManager<NetAppUser> signInManager,
        RoleManager<NetAppRole> roleManager,
        IOptions<JwtSettings> jwtSettings,
        IEmailService emailService,
        IRepositoryProvider repositoryProvider,
        ICurrentUserService currentUserService,
        IStringLocalizer<NetAppLocalizer> localizer,
        IDateTimeService dateTimeService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager=roleManager;
        _emailService = emailService;
        _repositoryProvider = repositoryProvider;
        _currentUserService=currentUserService;
        _localizer=localizer;
        _dateTimeService=dateTimeService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<IResponse<string>> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new ApiException(_localizer["Invalid confirmation token."]);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var authToken = await _repositoryProvider.AuthenticationTokenRepository.GetTokenByValueAsync(code);
        if (authToken == null)
            throw new ApiException(_localizer["Invalid confirmation token, link is broken."]);
        if (authToken.Expired)
        {
            _repositoryProvider.AuthenticationTokenRepository.DeleteToken(authToken);
            throw new ApiException(_localizer["Token has expired."]);
        }

        await HandleEmailConfirmationAsync(code, user, authToken);
        return Response<string>.Success(user.Email!, message: _localizer["Account confirmed."]);
    }

    private async Task HandleEmailConfirmationAsync(string code, NetAppUser user, AuthenticationToken authToken)
    {
        var result = await _userManager.ConfirmEmailAsync(user, code);
        _repositoryProvider.AuthenticationTokenRepository.DeleteToken(authToken);
        await _repositoryProvider.SaveChangesAsync();
        if (!result.Succeeded)
            throw new ApiException(_localizer["An error occurred."]);
    }

    public async Task<IResponse> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(_localizer["User not found."]);
        user.Active = false;
        await _userManager.UpdateAsync(user);
        return Response.Success(_localizer["User deleted successfully."]);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user == null) return;
        if (!user.EmailConfirmed)
            throw new ApiException($"Account not confirmed for '{request.Email}'.");
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var tokenKey = StringExtensions.GenerateRandomNumber(6);
        var emailRequest = new EmailRequest(request.Email!,
            _localizer["Reset Password"],
            $"Kindly use this token <strong>{tokenKey}</strong> to reset your password. The token is valid for <strong>5 mintues</strong>.");
        await _emailService.SendAsync(emailRequest);
        var authToken = new AuthenticationToken
        {
            Key = tokenKey,
            Value = resetToken,
            Expires = _dateTimeService.Now.AddMinutes(5),
            CreatedBy = request.Email!
        };

        await _repositoryProvider.AuthenticationTokenRepository.AddTokenAsync(authToken);
        await _repositoryProvider.SaveChangesAsync();
    }

    public async Task<IResponse<AuthenticationResponse>> GetRefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user == null)
            throw new NotFoundException("User Not Found.");
        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new ApiException("Invalid Client Token.");

        SetRefreshToken(user);
        await _userManager.UpdateAsync(user);
        return await GenerateAuthenticationResponse(user, ipAddress);
    }

    public async Task<IResponse<UserRolesResponse>> GetRolesAsync(string userId)
    {
        var viewModel = new List<UserRoleModel>();
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _roleManager.Roles.ToListAsync();
        roles = roles.Where(x => x.Name != ApplicationConstants.Role.SuperAdmin).ToList();
        foreach (var role in roles)
        {
            var selected = await _userManager.IsInRoleAsync(user!, role.Name!);
            viewModel.Add(new UserRoleModel(role.Name!, role.Description!, selected));
        }
        return Response<UserRolesResponse>.Success(new UserRolesResponse(viewModel));
    }

    public async Task<IResponse<IEnumerable<User>>> GetUsersAsync()
    {
        var superAdmin = (await _userManager.GetUsersInRoleAsync(ApplicationConstants.Role.SuperAdmin)).Select(u => u.Id).FirstOrDefault();
        var users = new List<User>();
        foreach (var user in _userManager.Users.Where(u => u.Id != superAdmin))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            users.Add(new User(user.Id, user.UserName!, user.Email!, userRoles, user.Active));
        }

        return Response<IEnumerable<User>>.Success(users);
    }

    public async Task<IResponse<AuthenticationResponse>> LoginAsync(AuthenticationRequest request, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new ApiException(_localizer["Invalid credentials."]);

        if (!user.Active)
            throw new ApiException(_localizer["User account is disabled."]);

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

        if (!result.Succeeded)
            throw new ApiException(_localizer["Invalid credentials."]);

        if (!user.EmailConfirmed)
            throw new ApiException(_localizer[$"Account not confirmed for '{request.Email}'."]);

        SetRefreshToken(user);

        await _userManager.UpdateAsync(user);
        return await GenerateAuthenticationResponse(user, ipAddress);
    }

    public async Task<IResponse<string>> RegisterAsync(RegisterRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user != null)
            throw new ApiException($"Email address '{request.Email}' already exists.");
        user = new NetAppUser
        {
            Email = request.Email,
            UserName = request.Username
        };
        var password = StringExtensions.GeneratePassword();
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new ApiException($"{result.Errors.First().Description}");
        await _userManager.AddToRoleAsync(user, ApplicationConstants.Role.Basic);
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id));
        await _userManager.AddClaimAsync(user, new Claim("username", user.UserName!));
        var verificationLink = await GenerateVerificationEmailLink(user, origin);
        try
        {
            string mailBody = $"<p>Dear {user.UserName},</p> <br/> <p>Your account has been created successfully.</p> <p>To login, use your email address and password: <strong>{password}</strong>.</p>" +
                            $" <br/> <p>Please click this <a href=\"{verificationLink}\" target=\"_blank\" >link</a> to confirm your account.</p>";
            await _emailService.SendAsync(new EmailRequest(user.Email!, _localizer["Confirm Registration"], mailBody));
        }
        catch (InvalidEmailAddressException)
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
        return Response<string>.Success(user.Id, message: _localizer["User registered."]);
    }

    public async Task ResendConfirmationMailAsync(ConfirmationMailRequest request, string ipAddress)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException(_localizer["User not found."]);

        var verificationLink = await GenerateVerificationEmailLink(user, ipAddress);
        string mailBody = $"<p>Please click this <a href=\"{verificationLink}\" target=\"_blank\" >link</a> to confirm your account.</p>";
        await _emailService.SendAsync(new EmailRequest(user.Email!, _localizer["Confirm Registration"], mailBody));
    }

    public async Task<IResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (!string.Equals(request.Password, request.ConfirmPassword))
            throw new ApiException(_localizer["The passwords entered does not match."]);
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user == null)
            throw new ApiException(_localizer["Invalid credentials."]);
        if (!user.Active)
            throw new ApiException(_localizer["User account is disabled."]);
        var authToken = await _repositoryProvider.AuthenticationTokenRepository.GetTokenByKeyAsync(request.Token!);
        if (authToken == null)
            throw new ApiException(_localizer["Token is either incorrect or expired."]);
        if (authToken.Expired || authToken.CreatedBy != request.Email)
            throw new ApiException(_localizer["Token is either incorrect or expired."]);

        var result = await _userManager.ResetPasswordAsync(user, authToken.Value, request.Password!);
        if (!result.Succeeded)
            throw new ApiException(result.Errors.First().Description ?? _localizer["Error occured while reseting the password."]);
        _repositoryProvider.AuthenticationTokenRepository.DeleteToken(authToken);
        await _repositoryProvider.SaveChangesAsync();
        return Response.Success(_localizer["Password reset successfully."]);
    }

    public async Task<IResponse> UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        if (!string.Equals(request.NewPassword, request.ConfirmPassword))
            throw new ApiException(_localizer["The passwords entered does not match."]);
        var user = await _userManager.FindByEmailAsync(_currentUserService.Email);
        if (user == null)
            throw new ApiException(_localizer["Something went wrong! Please login and try again."]);
        await HandlePasswordUpdateAsync(request, user);
        return Response.Success(_localizer["Password reset successfully."]);
    }

    private async Task HandlePasswordUpdateAsync(UpdatePasswordRequest request, NetAppUser user)
    {
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword!, request.NewPassword!);
        if (!result.Succeeded)
            throw new ApiException(result.Errors.First().Description ?? _localizer["Error occured while changing the password."]);
    }

    public async Task<IResponse> UpdateUserAsync(string id, EditUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");
        user.Active = request.Active;
        await _userManager.UpdateAsync(user);
        return Response.Success(_localizer["User updated successfully."]);
    }

    public async Task<IResponse> UpdateUserRoleAsync(string id, UpdateUserRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException(_localizer["User not found."]);

        if (id != request.UserId)
            throw new ApiException(_localizer["Invalid user id"]);

        if (user.Id == _currentUserService.UserId)
            throw new ApiException(_localizer["Operation not allowed."]);

        if (await _userManager.IsInRoleAsync(user, ApplicationConstants.Role.SuperAdmin))
            throw new ApiException(_localizer["Operation not allowed."]);

        var roles = await _userManager.GetRolesAsync(user);

        //if (!_currentUserService.IsAdmin && roles.Any(x => x == ApplicationConstants.Role.Admin))
        //    throw new ApiException("Not allowed to add or delete Administrator Role if you have not this role.");

        await HandleUserRoleUpdateAsync(request, user, roles);

        return Response.Success("User role updated successfully.");
    }

    private async Task HandleUserRoleUpdateAsync(UpdateUserRoleRequest request, NetAppUser user, IList<string> roles)
    {
        await _userManager.RemoveFromRolesAsync(user, roles);
        var result = await _userManager.AddToRoleAsync(user, request.SelectedRole.RoleName);
        if (!result.Succeeded)
            throw new ApiException(string.Join("|", result.Errors.Select(e => e.Description)));
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience =_jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        return IsValidToken(securityToken)
            ? throw new SecurityTokenException("Invalid token")
            : principal;
    }

    private static bool IsValidToken(SecurityToken securityToken) =>
        securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase);

    private void SetRefreshToken(NetAppUser user)
    {
        user.RefreshToken= StringExtensions.RandomTokenString();
        user.RefreshTokenExpiryTime = _dateTimeService.Now.AddDays(1);
    }

    private async Task<string> GenerateJWToken(NetAppUser user, IEnumerable<string> roles, string ipAddress)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var thisRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole!);
            permissionClaims.AddRange(allPermissionsForThisRoles);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("ip", ipAddress),
         }
        .Union(userClaims)
        .Union(roleClaims)
        .Union(permissionClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: _dateTimeService.Now.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    private async Task<IResponse<AuthenticationResponse>> GenerateAuthenticationResponse(NetAppUser user, string ipAddress)
    {
        var rolesList = await _userManager.GetRolesAsync(user);
        var jwtSecurityToken = await GenerateJWToken(user, rolesList, ipAddress);
        AuthenticationResponse response = new(jwtSecurityToken, user.RefreshToken!);
        return Response<AuthenticationResponse>.Success(response, _localizer[$"Authenticated {user.UserName}"]);
    }

    private async Task<string> GenerateVerificationEmailLink(NetAppUser user, string origin)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _repositoryProvider.AuthenticationTokenRepository.AddTokenAsync(new AuthenticationToken
        {
            Key = StringExtensions.GenerateRandomNumber(9),
            Value = code,
            Expires = _dateTimeService.Now.AddMinutes(30)
        });
        await _repositoryProvider.SaveChangesAsync().ConfigureAwait(false);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "users/confirm-email";
        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        return verificationUri;
    }

}

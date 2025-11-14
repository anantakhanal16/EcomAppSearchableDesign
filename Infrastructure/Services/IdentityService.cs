using System.Security.Claims;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Azure;
using Azure.Core;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class IdentityService(UserManager<User> userManager, IJwtTokenService jwtService, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager) : IIdentityService
{
    public async Task<ServiceResponseData<UserRegistrationResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var user = new User { UserName = request.email, Email = request.email, FullName = request.fullName };

        var result = await userManager.CreateAsync(user, request.password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return ServiceResponseData<UserRegistrationResponseDto>.Failure(errors);
        }

        return ServiceResponseData<UserRegistrationResponseDto>.Success("Registration successful");
    }

    public async Task<ServiceResponseData<UserLoginResponse>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.password)) return ServiceResponseData<UserLoginResponse>.Failure("Invalid Credentials");

        var accessToken = await jwtService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = await jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var userLoginResponse = new UserLoginResponse { accessToken = accessToken, refreshToken = refreshToken };
        var serviceResponse = new ServiceResponseData<UserLoginResponse> { Code = "0", Data = userLoginResponse, Message = "Login successful" };
        return serviceResponse;
    }

    public async Task<ServiceResponseData<User>> GetUserDetails(CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return new ServiceResponseData<User> { Code = "1", Message = "User not found", };
        }

        if (!user.Identity.IsAuthenticated)
        {
            return new ServiceResponseData<User> { Code = "1", Message = "User not authenticated" };
        }

        var email = user.FindFirst(ClaimTypes.Email)?.Value;
        var userDetail = await userManager.FindByEmailAsync(email);
        if (userDetail == null)
        {
            return ServiceResponseData<User>.Failure("Failed to get UserDetails");
        }

        return ServiceResponseData<User>.Success("User Details", userDetail);
    }

    public async Task<ServiceResponseData<UserLoginResponse>> GenerateRefreshToken(string token, CancellationToken cancellationToken)
    {
        var refreshToken = token;
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow, cancellationToken);

        if (user == null)
        {
            return ServiceResponseData<UserLoginResponse>.Failure("Invalid token");
        }

        var accessToken = await jwtService.GenerateAccessToken(user.Id, user.Email);

        UserLoginResponse userLoginResponse = new UserLoginResponse() { accessToken = accessToken };
        return ServiceResponseData<UserLoginResponse>.Success("Token refreshed", userLoginResponse);
    }
    public async Task<ServiceResponseData<string>> LogoutAsync(CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext?.User;

        if (principal == null || !principal.Identity.IsAuthenticated)
        {
            return ServiceResponseData<string>.Failure("User is not authenticated");
        }
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
            return ServiceResponseData<string>.Failure("Invalid user email");

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return ServiceResponseData<string>.Failure("User not found");


        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await userManager.UpdateSecurityStampAsync(user);
        await userManager.UpdateAsync(user);

        await signInManager.SignOutAsync();

        return ServiceResponseData<string>.Success("Logout successful");
    }
}
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomAppSearchableDesign.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AccountController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<HttpResponses<UserRegistrationResponseDto>> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {

        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<UserRegistrationResponseDto>();
        }
        var response = await _identityService.RegisterAsync(request, cancellationToken);
        if (response.Code != "0")
        {
            return HttpResponses<UserRegistrationResponseDto>.FailResponse(response.Message);
        }

        return HttpResponses<UserRegistrationResponseDto>.SuccessResponse(response.Data, response.Message);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<HttpResponses<UserLoginResponse>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
      
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<UserLoginResponse>();
        }

        var response = await _identityService.LoginAsync(request, cancellationToken);
        if (response.Code != "0")
        {
            return HttpResponses<UserLoginResponse>.FailResponse(response.Message);
        }

        return HttpResponses<UserLoginResponse>.SuccessResponse(response.Data, response.Message);
    }

    [HttpGet("getUserDetails")]
    public async Task<HttpResponses<User>> GetUserDetails(CancellationToken cancellationToken)
    {
        var userDetails = await _identityService.GetUserDetails(cancellationToken);
        if (userDetails.Code != "0")
        {
            return HttpResponses<User>.FailResponse(userDetails.Message);
        }

        return HttpResponses<User>.SuccessResponse(userDetails.Data, userDetails.Message);
    }

    [HttpPost("generaterefreshtoken")]
    [AllowAnonymous]
    public async Task<HttpResponses<UserLoginResponse>> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return HttpResponses<UserLoginResponse>.FailResponse("Failed to generate refresh token.");
        }

        var response = await _identityService.GenerateRefreshToken(refreshToken, cancellationToken);
        if (response.Code != "0")
        {
            return HttpResponses<UserLoginResponse>.FailResponse(response.Message);
        }

        return HttpResponses<UserLoginResponse>.SuccessResponse(response.Data, response.Message);
    }

    [HttpPost("logout")]
    public async Task<HttpResponses<string>> Logout(CancellationToken cancellationToken)
    {
        var response = await _identityService.LogoutAsync(cancellationToken);
        if (response.Code != "0")
        {
            return HttpResponses<string>.FailResponse(response.Message);
        }

        return HttpResponses<string>.SuccessResponse(null, response.Message);
    }
}
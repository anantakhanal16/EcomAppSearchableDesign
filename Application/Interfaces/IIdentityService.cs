using Application.Dtos;
using Application.Helpers;
using Core.Entities;

namespace Application.Interfaces
{
    public interface IIdentityService
    {
        Task<ServiceResponseData<UserRegistrationResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);
        Task<ServiceResponseData<UserLoginResponse>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
        Task<ServiceResponseData<User>> GetUserDetails(CancellationToken cancellationToken);
        Task<ServiceResponseData<UserLoginResponse>> GenerateRefreshToken(string token, CancellationToken cancellationToken);
        Task<ServiceResponseData<string>> LogoutAsync(CancellationToken cancellationToken);

    }
}
using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    string GenerateJwtToken(UserDto user);
}

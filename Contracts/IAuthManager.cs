using Azure;
using dotnet_notely.ModelDtos;
using Microsoft.AspNetCore.Identity;

namespace dotnet_notely.Contracts;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto);
    Task<AuthResponseDto> Login(LoginDto loginDto);
    Task<bool> Logout(string userId);
}
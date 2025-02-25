using System.Security.Claims;
using AutoMapper;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dotnet_notely.Repository;

public class UserRepository: IUserRepository
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserRepository(UserManager<ApiUser> userManager,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
        )
    {
        this._userManager = userManager;
        this._mapper = mapper;
        this._httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<List<ApiUser>> SearchUsers(string searchTerm, string currentUserId)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<ApiUser>(); 
        }

        return await _userManager.Users
            .Where(u => u.Id != currentUserId && 
                        (u.Email.Contains(searchTerm) || 
                         u.UserName.Contains(searchTerm) || 
                         u.FirstName.Contains(searchTerm) || 
                         u.LastName.Contains(searchTerm)))
            .Take(20) // Limit results to prevent performance issues
            .ToListAsync();
    }
    
    //
    public async Task<UserProfileDto> GetCurrentUserProfile()
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        return await GetUserProfileById(userId);
    }

    public async Task<UserProfileDto> GetUserProfileById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return _mapper.Map<UserProfileDto>(user);
    }
}
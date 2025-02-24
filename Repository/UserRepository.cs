using dotnet_notely.Contracts;
using dotnet_notely.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dotnet_notely.Repository;

public class UserRepository: IUserRepository
{
    private readonly UserManager<ApiUser> _userManager;

    public UserRepository(UserManager<ApiUser> userManager)
    {
        this._userManager = userManager;
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
}
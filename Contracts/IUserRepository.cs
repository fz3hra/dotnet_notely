using dotnet_notely.Data;
using dotnet_notely.ModelDtos;

namespace dotnet_notely.Contracts;

public interface IUserRepository
{
    Task<List<ApiUser>> SearchUsers(string searchTerm, string currentUserId);
    Task<UserProfileDto> GetCurrentUserProfile();
    Task<UserProfileDto> GetUserProfileById(string userId);
}
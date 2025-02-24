using dotnet_notely.Data;

namespace dotnet_notely.Contracts;

public interface IUserRepository
{
    Task<List<ApiUser>> SearchUsers(string searchTerm, string currentUserId);
}
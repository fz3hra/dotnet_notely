using Microsoft.AspNetCore.Identity;

namespace dotnet_notely.Data;

public class ApiUser: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // public string RefreshToken { get; set; }
    // public DateTime RefreshTokenExpiry { get; set; }
}
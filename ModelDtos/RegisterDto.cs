using System.ComponentModel.DataAnnotations;

namespace dotnet_notely.ModelDtos;

public class RegisterDto: LoginDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
}
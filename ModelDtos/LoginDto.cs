using System.ComponentModel.DataAnnotations;

namespace dotnet_notely.ModelDtos;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(15, ErrorMessage = "Your password must be between 6 and 15 characters long.")]
    public string Password { get; set; }
}
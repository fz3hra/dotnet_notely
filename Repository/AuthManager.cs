using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace dotnet_notely.Repository;

public class AuthManager : IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
    {
        this._mapper = mapper;
        this._userManager = userManager;
        this._configuration = configuration;
    }

    public async Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto)
    {
        var user = _mapper.Map<ApiUser>(registerDto);
        user.Email = registerDto.Email;
        user.FirstName = registerDto.FirstName;
        user.LastName = registerDto.LastName;
        user.UserName = registerDto.UserName;

        var res = await _userManager.CreateAsync(user, registerDto.Password);
        if (res.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return res.Errors;
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        bool isUserExist = false;
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        isUserExist = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (user == null || isUserExist == false)
        {
            return null;
        }
      
            // generate token then return data response.
            var token = await generateToken(user);
            return new AuthResponseDto
            {
                UserId=user.Id, Token=token
            };
    }

    // generate token for user:
    private async Task<string> generateToken(ApiUser user)
    {
        // create signing key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        
        // create crenditals
        var credentiais = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        // gather user information aka claims   
        var roles = await _userManager.GetRolesAsync(user);
        
        var roleClaims = roles.Select((role) => new Claim(ClaimTypes.Role, role));
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        
        // generate new Claims:
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        }.Union(userClaims).Union(userClaims);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentiais
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
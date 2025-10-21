using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using VirtualTour.Model;
using Microsoft.Extensions.Configuration;
namespace VirtualTour.BL.Services
{
    public interface ITokenService
    {
        string CreateToken(UserModel User, IList<string> userRole);
        // Create token including permissions
        string CreateToken(UserModel User);
    }
    public class TokenService: ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        }
        public string CreateToken(UserModel User, IList<string> userRole)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, User.Email),
                new Claim(ClaimTypes.Name, User.UserName),
                new Claim("full_name", User.FullName),
                new Claim("gender", User.Gender),
                new Claim("phone", User.PhoneNumber)
            };
            claims.AddRange(userRole.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.Add(new Claim("roleid", User.RoleId.ToString()));
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
       
        public string CreateToken(UserModel User)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, User.Email),
                new Claim(ClaimTypes.Name, User.UserName)
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

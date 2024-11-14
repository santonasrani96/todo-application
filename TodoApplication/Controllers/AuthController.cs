using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoApplication.Data;
using TodoApplication.Models;
using TodoApplication.Models.Entities;

namespace TodoApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration configuration;
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginRequestDto loginRequestDto)
        {
            var _user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == loginRequestDto.UserId);
            if (_user == null || !BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, _user.Password))
            {
                return BadRequest("User not found");
            }

            string _token = CreateToken(_user);
            return Ok(new { message = "Login Successfull", user = _user, token = _token });
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GenerateSecretKey()));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credential);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static string GenerateSecretKey(int keySize=256)
        {
            byte[] key = new byte[keySize / 8];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return Convert.ToBase64String(key);
        }
    }
}

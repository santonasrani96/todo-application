using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Models;
using TodoApplication.Models.Entities;

namespace TodoApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await dbContext.Users.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<List<User>>> CreateUser(AddUserDto userDto)
        {
            var user = new User()
            {
                UserId = userDto.UserId,
                Name = userDto.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Users.ToListAsync());
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<ActionResult<List<User>>> UpdateUser(Guid id, UpdateUserDto userDto)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.Name = userDto.Name;
            user.UserId = userDto.UserId;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }

            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Users.ToListAsync());
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<ActionResult<User>> DeleteUser(Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            dbContext.Users.Remove(user);

            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Users.ToListAsync());
        }
    }
}

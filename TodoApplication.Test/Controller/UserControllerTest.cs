using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Controllers;
using TodoApplication.Data;
using TodoApplication.Models;
using TodoApplication.Models.Entities;

namespace TodoApplication.Test.Controller
{
    public class UserControllerTest
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        public UserControllerTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task UsersController_GetUsers_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                await context.Users.AddRangeAsync(new List<User>
                {
                    new User { Id = Guid.NewGuid(), UserId = "user1", Password = "password1", Name = "User 1" },
                    new User { Id = Guid.NewGuid(), UserId = "user2", Password = "password2", Name = "User 2" },
                    new User { Id = Guid.NewGuid(), UserId = "user3", Password = "password3", Name = "User 3" },
                });

                await context.SaveChangesAsync();

                var controller = new UsersController(context);
                var result = await controller.GetUsers();

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var users = okResult?.Value as List<User>;
                users.Should().NotBeNull();
                users.Should().HaveCount(3);
                users[0].UserId.Should().Be("user1");
                users[1].Password.Should().Be("password2");
                users[2].Name.Should().Be("User 3");
            }
        }

        [Fact]
        public async Task UsersController_GetUser_ReturnOk()
        {
            using(var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                var userId = Guid.NewGuid();

                context.Users.Add(new User
                {
                    Id = userId,
                    UserId = "user1",
                    Password = "password1",
                    Name = "User 1"
                });

                await context.SaveChangesAsync();

                var controller = new UsersController(context);
                var result = await controller.GetUser(userId);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var user = okResult?.Value as User;
                user.Should().NotBeNull();
                user.Name.Should().Be("User 1");
            }
        }

        [Fact]
        public async Task UsersController_CreateUser_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                var userDto = new AddUserDto()
                {
                    UserId = "user4",
                    Name = "User 4",
                    Password = "password4"
                };

                var controller = new UsersController(context);
                var result = await controller.CreateUser(userDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var users = okResult?.Value as List<User>;
                users.Should().NotBeNull();
                users.Should().HaveCount(1);

                users[0].UserId.Should().Be(userDto.UserId);
                users[0].Name.Should().Be(userDto.Name);
                users[0].Password.Should().NotBe(userDto.Password);
                users[0].Password.Should().HaveLength(60);
                var verifyPassword = BCrypt.Net.BCrypt.Verify(userDto.Password, users[0].Password);
                verifyPassword.Should().BeTrue();
            }
        }

        [Fact]
        public async Task UsersController_UpdateUser_ReturnOk()
        {
            using(var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                var userId = Guid.NewGuid();
                var userDto = new UpdateUserDto()
                {
                    UserId = "userUpdated1",
                    Name = "User Updated 1",
                    Password = "passwordUpdated1"
                };

                context.Users.Add(new User()
                {
                    Id = userId,
                    UserId = "user1",
                    Name = "User 1",
                    Password = BCrypt.Net.BCrypt.HashPassword("password1")
                });

                await context.SaveChangesAsync();

                var controller = new UsersController(context);
                var result = await controller.UpdateUser(userId, userDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var users = okResult?.Value as List<User>;
                users.Should().NotBeNull();
                users.Should().HaveCount(1);

                var updatedUser = users[0];
                updatedUser.UserId.Should().Be(userDto.UserId);
                updatedUser.Name.Should().Be(userDto.Name);
                updatedUser.Password.Should().NotBe("passwordUpdated1");
                updatedUser.Password.Should().HaveLength(60);
                var verifyPassword = BCrypt.Net.BCrypt.Verify(userDto.Password, updatedUser.Password);
                verifyPassword.Should().BeTrue();
            }
        }

        [Fact]
        public async Task UsersController_UpdateUser_ReturnBadRequest()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                var userId = Guid.NewGuid();
                var userDto = new UpdateUserDto
                {
                    UserId = "updatedUserId",
                    Name = "Updated User",
                    Password = "updatedPassword"
                };

                var controller = new UsersController(context);

                var result = await controller.UpdateUser(userId, userDto);

                var badRequestResult = result.Result as BadRequestObjectResult;
                badRequestResult.Should().NotBeNull();
                badRequestResult.Value.Should().Be("User not found");
            }
        }

        [Fact]
        public async Task UsersController_DeleteUser_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                var userId1 = Guid.NewGuid();
                var userId2 = Guid.NewGuid();
                context.Users.Add(new User()
                {
                    Id = userId1,
                    UserId = "user1",
                    Name = "User 1",
                    Password = "password1"
                });

                context.Users.Add(new User()
                {
                    Id = userId2,
                    UserId = "user2",
                    Name = "User 2",
                    Password = "password2"
                });

                await context.SaveChangesAsync();

                var controller = new UsersController(context);
                var result = await controller.DeleteUser(userId1);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var users = okResult?.Value as List<User>;
                users.Should().NotBeNull();
                users.Should().HaveCount(1);
                users[0].UserId.Should().Be("user2");

                var deletedUser = await context.Users.FindAsync(userId1);
                deletedUser.Should().BeNull();
            }
        }

        [Fact]
        public async Task UsersController_DeleteUser_ReturnBadRequest()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Users.RemoveRange(context.Users);
                await context.SaveChangesAsync();

                var userIdNotExisting = Guid.NewGuid();

                var userId = Guid.NewGuid();
                context.Users.Add(new User()
                {
                    Id = userId,
                    UserId = "user1",
                    Name = "User 1",
                    Password = "password1"
                });

                await context.SaveChangesAsync();

                var controller = new UsersController(context);
                var result = await controller.DeleteUser(userIdNotExisting);

                var badRequestResult = result.Result as BadRequestObjectResult;
                badRequestResult.Should().NotBeNull();
                badRequestResult?.Value.Should().Be("User not found");

                var users = await context.Users.ToListAsync();
                users.Should().HaveCount(1);
                users.Should().NotBeEmpty();
            }
        }
    }
}

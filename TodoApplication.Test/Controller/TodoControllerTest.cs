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
    public class TodoControllerTest
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        public TodoControllerTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task TodosController_GetTodos_ReturnOk()
        {
            using(var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                await context.Todos.AddRangeAsync(new List<Todo>
                {
                    new Todo { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ActivitiesNo = "AC-0001", Subject = "Todo 1", Description = "Description 1", Status = null},
                    new Todo { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ActivitiesNo = "AC-0002", Subject = "Todo 2", Description = "Description 2", Status = 1},
                    new Todo { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ActivitiesNo = "AC-0003", Subject = "Todo 3", Description = "Description 3", Status = 2}
                });

                await context.SaveChangesAsync();

                var controller = new TodosController(context);
                var result = await controller.GetTodos();

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var todos = okResult?.Value as List<Todo>;
                todos.Should().NotBeNull();
                todos.Should().HaveCount(3);
                todos[0].ActivitiesNo.Should().Be("AC-0001");
                todos[1].Subject.Should().Be("Todo 2");
                todos[2].Description.Should().Be("Description 3");
            }
        }

        [Fact]
        public async Task TodosController_GetTodo_ReturnOk()
        {
            var todoId = Guid.NewGuid();
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                context.Todos.Add(new Todo
                {
                    Id = todoId,
                    UserId = Guid.NewGuid(),
                    ActivitiesNo = "AC-0005",
                    Subject = "Subject 1",
                    Description = "Description 1"
                });

                await context.SaveChangesAsync();

                var controller = new TodosController(context);
                var result = await controller.GetTodo(todoId);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var todo = okResult?.Value as Todo;
                todo.Should().NotBeNull();
                todo.Subject.Should().Be("Subject 1");
            }
        }

        [Fact]
        public async Task TodosController_GetTodosByUserId_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var userId = Guid.NewGuid();

                context.Todos.Add(new Todo
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ActivitiesNo = "AC-0001",
                    Subject = "Subject 1",
                    Description = "Description 1"
                });

                context.Todos.Add(new Todo
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ActivitiesNo = "AC-0002",
                    Subject = "Subject 2",
                    Description = "Description 2"
                });

                await context.SaveChangesAsync();

                var controller = new TodosController(context);

                var result = await controller.GetTodosByUserId(userId);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var todos = okResult?.Value as IEnumerable<Todo>;
                todos.Should().NotBeNull();
                todos.Should().HaveCount(2);
                todos.Should().Contain(todo => todo.Subject == "Subject 1");
                todos.Should().Contain(todo => todo.Subject == "Subject 2");
            }
        }

        [Fact]
        public async Task TodoController_CreateTodo_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                await context.Todos.AddAsync(new Todo
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ActivitiesNo = "AC-0001",
                    Subject = "Subject 1",
                    Description = "Description 1"
                });

                var todoDto = new AddTodoDto()
                {
                    UserId = Guid.NewGuid(),
                    Subject = "Subject 1",
                    Description = "Description 1"
                };

                var controller = new TodosController(context);
                var result = await controller.CreateTodo(todoDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var todos = okResult?.Value as List<Todo>;
                todos.Should().NotBeNull();
                todos.Should().HaveCount(2);

                todos[0].Subject.Should().Be(todoDto.Subject);
                todos[0].Description.Should().Be(todoDto.Description);
            }
        }

        [Fact]
        public async Task TodosController_CreateBatchTodo_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var todosDto = new List<AddTodoDto>
                {
                    new AddTodoDto { UserId = Guid.NewGuid(), Subject = "Subject 1", Description = "Description 1", Status = null },
                    new AddTodoDto { UserId = Guid.NewGuid(), Subject = "Subject 2", Description = "Description 2", Status = null },
                    new AddTodoDto { UserId = Guid.NewGuid(), Subject = "Subject 3", Description = "Description 3", Status = null }
                };

                var controller = new TodosController(context);
                var result = await controller.CreateBatchTodo(todosDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var todos = okResult?.Value as List<Todo>;
                todos.Should().NotBeNull();
                todos.Should().HaveCount(3);

                todos[0].Subject.Should().Be("Subject 1");
                todos[1].Description.Should().Be("Description 2");
                todos[2].Status.Should().BeNull();
            }
        }

        [Fact]
        public async Task TodosController_UpdateTodo_ReturnsOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var todoId = Guid.NewGuid();
                await context.Todos.AddAsync(new Todo
                {
                    Id = todoId,
                    ActivitiesNo = "AC-0001",
                    Subject = "Subject 1",
                    Description = "Description 1",
                    Status = null
                });
                await context.SaveChangesAsync();

                var updateDto = new UpdateTodoDto
                {
                    Subject = "Updated Subject",
                    Description = "Updated Description",
                    Status = 1
                };

                var controller = new TodosController(context);
                var result = await controller.UpdateTodo(todoId, updateDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var todos = okResult?.Value as List<Todo>;
                todos.Should().NotBeNull();
                todos.Should().HaveCount(1);

                var updatedTodo = todos[0];
                updatedTodo.Subject.Should().Be("Updated Subject");
                updatedTodo.Description.Should().Be("Updated Description");
                updatedTodo.Status.Should().Be(1);
            }
        }

        [Fact]
        public async Task TodosController_DeleteTodo_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var todoId = Guid.NewGuid();
                await context.Todos.AddAsync(new Todo
                {
                    Id = todoId,
                    ActivitiesNo = "AC-0001",
                    Subject = "Subject 1",
                    Description = "Description 1",
                    Status = null
                });
                await context.SaveChangesAsync();

                var controller = new TodosController(context);
                var result = await controller.DeleteTodo(todoId);

                var okResult = result.Result as OkResult;
                okResult.Should().NotBeNull();

                var deletedTodo = await context.Todos.FindAsync(todoId);
                deletedTodo.Should().BeNull();
            }
        }

        [Fact]
        public async Task TodosController_DeleteTodo_ReturnBadRequest()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var todoId = Guid.NewGuid();

                var controller = new TodosController(context);
                var result = await controller.DeleteTodo(todoId);

                var badRequestResult = result.Result as BadRequestObjectResult;
                badRequestResult.Should().NotBeNull();
                badRequestResult.Value.Should().Be("Todo not found");
            }
        }

        [Fact]
        public async Task TodosController_UpdateStatusTodo_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var todoId = Guid.NewGuid();
                await context.Todos.AddAsync(new Todo
                {
                    Id = todoId,
                    ActivitiesNo = "AC-0001",
                    Subject = "Subject 1",
                    Description = "Description 1",
                    Status = null
                });
                await context.SaveChangesAsync();

                var statusDto = new UpdateTodoStatusDto { Status = 1 };

                var controller = new TodosController(context);
                var result = await controller.UpdateStatusTodo(todoId, statusDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var updatedTodo = await context.Todos.FindAsync(todoId);
                updatedTodo.Should().NotBeNull();
                updatedTodo.Status.Should().Be(1);
            }
        }

        [Fact]
        public async Task TodosController_UpdateStatusTodo_ReturnBadRequest()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var nonExistentTodoId = Guid.NewGuid();

                var statusDto = new UpdateTodoStatusDto { Status = 1 };

                var controller = new TodosController(context);
                var result = await controller.UpdateStatusTodo(nonExistentTodoId, statusDto);

                var badRequestResult = result.Result as BadRequestObjectResult;
                badRequestResult.Should().NotBeNull();
                badRequestResult.Value.Should().Be("Todo not found");
            }
        }

        [Fact]
        public async Task TodosController_UpdateBatchStatusTodo_ReturnOk()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var todoIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

                await context.Todos.AddRangeAsync(new List<Todo>
                {
                    new Todo { Id = todoIds[0], ActivitiesNo = "AC-0001", Subject = "Subject 1", Description = "Description 1", Status = null },
                    new Todo { Id = todoIds[1], ActivitiesNo = "AC-0002", Subject = "Subject 2", Description = "Description 2", Status = null },
                    new Todo { Id = todoIds[2], ActivitiesNo = "AC-0003", Subject = "Subject 3", Description = "Description 3", Status = null }
                });

                await context.SaveChangesAsync();

                var updateDto = new UpdateBatchTodoStatusDto
                {
                    Ids = todoIds,
                    Status = 0
                };

                var controller = new TodosController(context);
                var result = await controller.UpdateBatchStatusTodo(updateDto);

                var okResult = result.Result as OkObjectResult;
                okResult.Should().NotBeNull();

                var updatedTodos = await context.Todos.Where(todo => todoIds.Contains(todo.Id)).ToListAsync();
                updatedTodos.Should().HaveCount(3);
                updatedTodos.Should().OnlyContain(todo => todo.Status == 0);
            }
        }

        [Fact]
        public async Task TodosController_UpdateBatchStatusTodo_ReturnBadRequest()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Todos.RemoveRange(context.Todos);
                await context.SaveChangesAsync();

                var updateDto = new UpdateBatchTodoStatusDto
                {
                    Ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                    Status = 1
                };

                var controller = new TodosController(context);
                var result = await controller.UpdateBatchStatusTodo(updateDto);

                var badRequestResult = result.Result as BadRequestObjectResult;
                badRequestResult.Should().NotBeNull();
                badRequestResult.Value.Should().Be("No todos found for the provided IDs.");
            }
        }

    }
}

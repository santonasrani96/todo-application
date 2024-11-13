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
    public class TodosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public TodosController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetTodos()
        {
            return Ok(await dbContext.Todos.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<Todo>> GetTodo(Guid id)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
            {
                return BadRequest("Todo not found");
            }

            return Ok(todo);
        }

        [HttpGet]
        [Route("user/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosByUserId(Guid userId)
        {
            var todos = await dbContext.Todos.Where(t => t.UserId == userId).ToListAsync();
            return Ok(todos);
        }

        [HttpPost]
        public async Task<ActionResult<List<Todo>>> CreateTodo(AddTodoDto todoDto)
        {
            var lastActivitiesNo = await dbContext.Todos.OrderByDescending(x => x.ActivitiesNo).FirstOrDefaultAsync();

            string newActivitiesNo;

            if (lastActivitiesNo == null)
            {
                newActivitiesNo = "AC-0001";
            }
            else
            {
                var lastNumber = lastActivitiesNo.ActivitiesNo.Substring(3);
                int newNumber = int.Parse(lastNumber) + 1;

                newActivitiesNo = $"AC-{newNumber:0000}";
            }

            var todo = new Todo()
            {
                UserId = todoDto.UserId,
                ActivitiesNo = newActivitiesNo,
                Subject = todoDto.Subject,
                Description = todoDto.Description,
                Status = todoDto.Status
            };

            dbContext.Todos.Add(todo);
            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Todos.ToListAsync());
        }

        [HttpPost]
        [Route("create/batch")]
        public async Task<ActionResult<List<Todo>>> CreateBatchTodo(List<AddTodoDto> todosDto)
        {
            if (todosDto == null || !todosDto.Any())
            {
                return BadRequest("No todo to create");
            }

            var lastActivitiesNo = await dbContext.Todos.OrderByDescending(x => x.ActivitiesNo).FirstOrDefaultAsync();
            int lastNumber = lastActivitiesNo == null ? 1 : int.Parse(lastActivitiesNo.ActivitiesNo.Substring(3)) + 1;

            var todos = new List<Todo>();
            foreach (var todoDto in todosDto)
            {
                string newActivitiesNo = $"AC-{lastNumber:0000}";
                var todo = new Todo()
                {
                    Id = Guid.NewGuid(),
                    ActivitiesNo = newActivitiesNo,
                    Subject = todoDto.Subject,
                    Description = todoDto.Description,
                    Status = todoDto.Status
                };

                todos.Add(todo);
                lastNumber++;
            }

            await dbContext.Todos.AddRangeAsync(todos);
            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Todos.ToListAsync());
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<ActionResult<List<Todo>>> UpdateTodo(Guid id, UpdateTodoDto todoDto)
        {
            var todo = await dbContext.Todos.FindAsync(id);

            if (todo == null)
            {
                return BadRequest("Todo not found");
            }

            todo.Subject = todoDto.Subject;
            todo.Description = todoDto.Description;
            todo.Status = todoDto.Status;

            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Todos.ToListAsync());
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<ActionResult<Todo>> DeleteTodo(Guid id)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
            {
                return BadRequest("Todo not found");
            }

            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("status/{id:guid}")]
        public async Task<ActionResult<List<Todo>>> UpdateStatusTodo(Guid id, UpdateTodoStatusDto statusDto)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
            {
                return BadRequest("Todo not found");
            }

            todo.Status = statusDto.Status;
            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Todos.ToListAsync());
        }

        [HttpPut]
        [Route("status/batch")]
        public async Task<ActionResult<List<Todo>>> UpdateBatchStatusTodo(UpdateBatchTodoStatusDto todosDto)
        {
            if (todosDto?.Ids == null || !todosDto.Ids.Any())
            {
                return BadRequest("No todos to update.");
            }

            var todosToUpdate = await dbContext.Todos
                .Where(todo => todosDto.Ids.Contains(todo.Id))
                .ToListAsync();

            if (todosToUpdate.Count == 0)
            {
                return BadRequest("No todos found for the provided IDs.");
            }

            foreach (var todo in todosToUpdate)
            {
                todo.Status = todosDto.Status;
            }

            await dbContext.SaveChangesAsync();

            return Ok(await dbContext.Todos.ToListAsync());
        }
    }
}

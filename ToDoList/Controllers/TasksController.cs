using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using ToDoList.Data;
using ToDoList.DTO;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbCtx _ctx;

        public TasksController(AppDbCtx ctx)
        {
            _ctx = ctx;
        }

        [HttpPost("task")]
        [Authorize]
        public async Task<IActionResult> AddTask(TaskDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized();
            }
            var user = _ctx.Users.FirstOrDefault(u => u.Email == userEmail)!;
            var task = new TaskItem()
            {
                UserId = user.Id.ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                DueBy = dto.DueBy
            };

            _ctx.Tasks.Add(task);
            await _ctx.SaveChangesAsync();
            return Ok("Task saved successfully");
        }

        [HttpPost("tasks")]
        [Authorize]
        public async Task<IActionResult> AddTaskRange(IFormFile data)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized();
            }
            var user = _ctx.Users.FirstOrDefault(u => u.Email == userEmail)!;

            if (data == null || data.Length == 0)
            {
                return BadRequest("file not uploaded or is empty");
            }

            using (var reader = new StreamReader(data.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var tasks = csv.GetRecords<TaskItem>().ToList();
                if (!tasks.Any())
                {
                    return BadRequest("issue converting csv file to readable data");
                }

                foreach(var task in tasks)
                {
                    task.UserId = user.Id.ToString();
                    _ctx.Tasks.Add(task);
                }
                
                await _ctx.SaveChangesAsync();
                return Ok("Tasks added successfully");
            }
        }

        [HttpGet("tasks")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized();
            }
            var user = _ctx.Users.FirstOrDefault(u => u.Email == userEmail)!;

            var tasks = await _ctx.Tasks.Where(t => t.UserId == user.Id.ToString()).ToListAsync();

            if (!tasks.Any())
            {
                return NotFound("No tasks found for current user");
            }

            return Ok(tasks);
        }

        [HttpGet("task")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTask(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized();
            }
            var user = _ctx.Users.FirstOrDefault(u => u.Email == userEmail)!;

            var task = await _ctx.Tasks.FirstOrDefaultAsync(t => t.Id.ToString() == id && t.UserId == user.Id.ToString());

            if (task == null)
            {
                return NotFound("No task found with specified ID");
            }

            return Ok(task);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateTask(string id, UpTaskDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized();
            }
            var user = _ctx.Users.FirstOrDefault(u => u.Email == userEmail)!;

            var task = await _ctx.Tasks.FirstOrDefaultAsync(t => t.Id.ToString() == id && t.UserId == user.Id.ToString());
            if (task == null)
            {
                return NotFound("The Task was not found");
            }

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.DueBy = dto.DueBy;

            await _ctx.SaveChangesAsync();
            return Ok("Task Updated successfully");
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized();
            }
            var user = _ctx.Users.FirstOrDefault(u => u.Email == userEmail)!;

            var task = await _ctx.Tasks.FirstOrDefaultAsync(t => t.Id.ToString() == id && t.UserId == user.Id.ToString());
            if (task == null)
            {
                return NotFound("The Task was not found");
            }

            _ctx.Remove(task);
            await _ctx.SaveChangesAsync();
            return Ok("Task deleted successfully");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoList.Data;
using ToDoList.DTO;
using ToDoList.Migrations;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AppDbCtx _ctx;
        private readonly IConfiguration _config;

        public AuthController(AppDbCtx ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegDto dto)
        {
            //Checking if the email exists in the database. If it does API returns 400 bad request 
            var exists = await _ctx.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
            {
                return BadRequest("Email already exists");
            }


            //creating a new object that will take the data from the dto to an object that will be used to store into the database
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            //adds the user object to the dbSet object Users
            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync(); //updates the database to store the new data


            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = Authenticate(dto);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = Generate(user);
            return Ok(token);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<ActionResult> Delete(string email)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail != email)
            {
                return Unauthorized("Token Invalid");
            }


            var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound("User could not be found");
            }

            var tasks = await _ctx.Tasks.Where(t => t.UserId == user.Id.ToString()).ToListAsync();
            if (tasks.Count > 0)
            {
                _ctx.Tasks.RemoveRange(tasks);
                await _ctx.SaveChangesAsync();
            }

            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
            return Ok("User safely deleted");
        }
        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };

            var token = new JwtSecurityToken(
               issuer: _config["Jwt:Issuer"],
               audience: _config["Jwt:Audience"],
               claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(LoginDTO dto)
        {
            var current = _ctx.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (current != null)
            {
                var valid = BCrypt.Net.BCrypt.Verify(dto.Password, current.PasswordHash);
                if (valid)
                {
                    return current;
                }
            }
            return null;
        }

    }
}

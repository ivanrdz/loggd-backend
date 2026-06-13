using Loggd.Domain.Entities;
using Loggd.Infrastructure.Data;
using Loggd.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loggd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
    {
        try
        {
            var result = await authService.AuthenticateWithGoogleAsync(request.IdToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("dev-login")]
    public async Task<IActionResult> DevLogin(
        [FromServices] AppDbContext db,
        [FromServices] JwtService jwtService)
    {
        // Solo funciona en desarrollo
        if (!HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsDevelopment())
            return NotFound();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "dev@loggd.test");

        if (user is null)
        {
            user = new User
            {
                Email = "dev@loggd.test",
                Name = "Dev User",
                Level = 1,
                TotalXP = 0
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var token = jwtService.GenerateToken(user.Id, user.Email);
        return Ok(new { accessToken = token, userId = user.Id });
    }
}

public record GoogleAuthRequest(string IdToken);
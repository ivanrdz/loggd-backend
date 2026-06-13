using Google.Apis.Auth;
using Loggd.Domain.Entities;
using Loggd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Loggd.Infrastructure.Services;

public class AuthService(AppDbContext db, JwtService jwtService)
{
    public async Task<object> AuthenticateWithGoogleAsync(string idToken)
    {
        // Valida el token con Google
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        }
        catch
        {
            throw new UnauthorizedAccessException("Token de Google inválido.");
        }

        // Busca o crea el usuario
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user is null)
        {
            user = new User
            {
                Email = payload.Email,
                Name = payload.Name ?? payload.Email,
                AvatarUrl = payload.Picture,
                GoogleId = payload.Subject,
            };
            db.Users.Add(user);
        }
        else
        {
            user.LastActiveAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        var token = jwtService.GenerateToken(user.Id, user.Email);

        return new
        {
            accessToken = token,
            user = new
            {
                user.Id,
                user.Email,
                user.Name,
                user.AvatarUrl,
                user.Level,
                user.TotalXP
            }
        };
    }
}
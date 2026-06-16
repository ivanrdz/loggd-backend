# Loggd — Backend API

REST API for Loggd, a full-stack life tracking app. Built with .NET 9, Entity Framework Core and PostgreSQL.

🔗 **Swagger UI:** https://loggd-backend-production.up.railway.app/swagger  
🔗 **Frontend:** https://loggd-web.vercel.app

---

## Features

- JWT authentication with Google OAuth
- Habits module — check-ins, streak calculation, contribution grid
- Goals module — progress tracking, auto-complete on target reached
- Tasks module — priorities, tags, due dates, recurring tasks
- Auto database migrations on startup
- CORS configured for Angular frontend

## Tech Stack

| Layer | Tech |
|---|---|
| Framework | .NET 9 / ASP.NET Core |
| ORM | Entity Framework Core 9 |
| Database | PostgreSQL 17 |
| Auth | JWT Bearer + Google Identity |
| Docs | Swagger / OpenAPI |
| Deploy | Railway |

## Architecture

Loggd.Domain/          # Entities — User, Habit, Goal, Task
Loggd.Application/     # Interfaces and DTOs
Loggd.Infrastructure/  # DbContext, Services, EF migrations
Loggd.API/             # Controllers, Program.cs

Clean architecture with 4 layers. Each layer only references the one below it.

## API Endpoints

### Auth
| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| POST | /api/Auth/google | No | Login or register with Google |
| GET | /api/Auth/me | Yes | Current user info |

### Habits
| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| GET | /api/Habits | Yes | Get all habits |
| POST | /api/Habits | Yes | Create habit |
| POST | /api/Habits/{id}/checkin | Yes | Daily check-in |
| GET | /api/Habits/{id}/contributions | Yes | Yearly contribution grid |
| DELETE | /api/Habits/{id} | Yes | Archive habit |

### Goals
| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| GET | /api/Goals | Yes | Get all goals |
| POST | /api/Goals | Yes | Create goal |
| POST | /api/Goals/{id}/progress | Yes | Log progress |

### Tasks
| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| GET | /api/Tasks | Yes | Get tasks |
| POST | /api/Tasks | Yes | Create task |
| POST | /api/Tasks/{id}/complete | Yes | Toggle complete |
| DELETE | /api/Tasks/{id} | Yes | Delete task |

## Run locally

```bash
# Requirements: .NET 9 SDK, PostgreSQL

# Update connection string in appsettings.json

# Apply migrations
dotnet ef database update --project Loggd.Infrastructure --startup-project Loggd.API

# Run
dotnet run --project Loggd.API

# Swagger UI at http://localhost:5173/swagger
```

## Environment Variables (Railway)

ConnectionStrings__DefaultConnection = postgresql://...
Jwt__Secret = your-secret-key
Jwt__Issuer = loggd-api
Jwt__ExpirationDays = 30

---

Built by [Ivan Rodriguez](https://github.com/ivanrdz) — WAIA.MX
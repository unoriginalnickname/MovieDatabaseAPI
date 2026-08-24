# MovieDatabaseAPI

An ASP.NET Core Web API over a movie database, built with EF Core and ASP.NET Identity.
Course project, .NET 10. The solution file is `EFCorePracticeProject.slnx`.

The domain is small on purpose — movies, actors, genres, reviews — and the point of the
project is the layering, not the subject.

## Solution layout

Six projects plus a separate test project. Each one depends only on the row below it.

| Project | What it holds |
|---|---|
| `MovieApi` | The host. `Program.cs`, DI wiring, JWT setup, middleware, OpenAPI. |
| `MoviePresentation` | Controllers. Referenced by the host as an application part, so routing lives outside the startup project. |
| `MovieServices` | Service implementations, AutoMapper profile, paging, the `ServiceResult` factory. |
| `MovieContracts` | The service interfaces the presentation layer talks to. |
| `MovieData` | `MovieDbContext`, repositories, unit of work, migrations, seed data. |
| `MovieCore` | Entities, DTOs, query objects, repository interfaces, the Identity user. |
| `MovieApi.Test` | Test project: xUnit, Moq, AutoFixture, EF Core InMemory. |

Controllers depend on `IServiceManager`, never on a repository or the `DbContext`.
Services depend on `IUnitOfWork`. Only `MovieData` knows EF Core is involved.

Errors do not travel as exceptions between layers. A service returns `ServiceResult`
or `ServiceResult<T>` carrying an `ErrorTypeEnum`, and `ControllerExtensions.MapResult`
turns that into the right status code. `ExceptionMiddleware` catches what gets past it
and returns a ProblemDetails response.

## Running it

Needs the .NET 10 SDK and a SQL Server instance. The default connection string in
`MovieApi/appsettings.json` points at `Server=.` with a trusted connection.

```
dotnet run --project MovieApi
```

The root path redirects to `/scalar`, which serves the API reference from the generated
OpenAPI document.

> **In Development the database is dropped on every start.** `Program.cs` calls
> `EnsureDeletedAsync()`, then `MigrateAsync()`, then seeds. That is deliberate for a
> course project and it is the only mode that seeds data, but do not point it at a
> database you want to keep.

Outside Development, apply migrations yourself:

```
dotnet ef database update --project MovieData --startup-project MovieApi
```

There is one migration, `InitialCreate`. It squashes the earlier history, so an existing
database has to be recreated rather than upgraded — the rows in `__EFMigrationsHistory`
name migrations that no longer exist.

## Endpoints

| Route | Notes |
|---|---|
| `POST /api/auth/register`, `POST /api/auth/login` | Register creates the user; login returns a JWT. `GET /api/auth/profile` requires it. |
| `GET /api/movies` | Search and paging. Filters on title, year, language, genre, actor. |
| `GET /api/movies/{id}` | Summary plus links to itself, its reviews and its delete route. |
| `GET /api/movies/{id}/details` | The full record, with genres, actors and reviews. |
| `POST`/`PUT`/`PATCH`/`DELETE /api/movies/{id}` | `PATCH` takes a JSON Patch document. |
| `PUT /api/movies/{id}/actors`, `/genres` | Set the relations. `POST /api/movies/{id}/actors/{actorId}` adds one. |
| `GET`/`POST`/`PUT`/`DELETE /api/actors`, `/api/genres` | Straight CRUD. |
| `GET /api/reviews`, `/api/reviews/{id}`, `/api/movies/{id}/reviews` | Reviews are created under a movie and read either way. |

Paging is `?page=` and `?pageSize=`, defaulting to page 1 and 10, capped at 100. Paged
responses carry `TotalItems`, `CurrentPage`, `PageSize` and `TotalPages`.

## Auth

ASP.NET Identity over the same `DbContext`, with JWT bearer tokens. Passwords need eight
characters, a digit and an uppercase letter. Issuer, audience and signing key come from
the `JwtSettings` section; the value committed to `appsettings.json` is a placeholder and
belongs in user secrets or the environment for anything real.

## Status

`dotnet build` on the solution is clean. `MovieApi/Dockerfile` is the one Visual Studio
generates and has not been run.

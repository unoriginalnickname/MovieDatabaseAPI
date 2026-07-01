using Asp.Versioning;
using EFCorePracticeProject.Data;
using EFCorePracticeProject.Mappings;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

#region Controllers + JSON
builder.Services.AddControllers()
    .AddApplicationPart(typeof(MoviesController).Assembly)
    .AddNewtonsoftJson();
#endregion

#region API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc();
#endregion

#region DbContext
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());
#endregion

#region Scalar / OpenAPI (O

builder.Services.AddOpenApi();

#endregion

#region DI Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IMovieRelationService, MovieRelationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IActorService, ActorService>();



builder.Services.AddAutoMapper(cfg =>
    cfg.AddProfile<MappingProfile>());
#endregion

#region ProblemDetails + Exception Handling
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

#endregion

var app = builder.Build();



app.Logger.LogInformation("Starting application");

#region Middleware
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
#endregion

#region Root redirect
app.MapGet("/", () => Results.Redirect("/scalar"));
#endregion

#region Development only
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // OpenAPI endpoint (required for Scalar)
    app.MapOpenApi();

    // Scalar UI
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

    await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
    await app.SeedData();
}
#endregion

#region Test endpoint
app.MapGet("/Test", async (ILogger<Program> logger, HttpResponse response) =>
{
    logger.LogInformation("Test endpoint hit");
    await response.WriteAsync("Testing");
});
#endregion

app.Logger.LogInformation("App running");
app.Run();
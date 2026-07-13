using EFCorePracticeProject.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Diagnostics;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
    })
    .AddEntityFrameworkStores<MovieDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings["Secret"]!
                        ))
            };
    });
builder.Services.AddAuthorization();


#region Controllers + JSON
builder.Services.AddControllers()
    .AddApplicationPart(typeof(MoviesController).Assembly)
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling =
            Newtonsoft.Json.NullValueHandling.Ignore; //ignores null, otherwise will send empty error messages
        options.SerializerSettings.DefaultValueHandling =
            Newtonsoft.Json.DefaultValueHandling.Ignore;
    });

#endregion

#region OpenAPI
builder.Services.AddOpenApi();
#endregion

#region DbContext
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());
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

#region ProblemDetails
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd(
            "requestId",
            context.HttpContext.TraceIdentifier);

        Activity? activity =
            context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});
#endregion

var app = builder.Build();

app.Logger.LogInformation("Starting application");

#region Middleware
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseAuthentication();


app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
#endregion

#region Root redirect
app.MapGet("/", () => Results.Redirect("/scalar"));
#endregion

#region OpenAPI
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.MapOpenApi();

    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

    await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
    await app.SeedData();
}
#endregion


app.Logger.LogInformation("App running");
app.Run();
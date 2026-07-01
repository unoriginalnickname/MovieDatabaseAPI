using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EFCorePracticeProject.Data;
using EFCorePracticeProject.Mappings;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

#region Controllers + JSON
builder.Services.AddControllers()
    .AddApplicationPart(typeof(MoviesController).Assembly)
    .AddNewtonsoftJson();
#endregion

#region API Versioning
builder.Services.AddApiVersioning(
        options =>
        {
            // reporting api versions will return the headers
            // "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;

            options.Policies.Deprecate(0.9)
                .Effective(DateTimeOffset.Now)
                .Link("policy.html")
                .Title("Version Deprecation Policy")
                .Type("text/html");

            options.Policies.Sunset(0.9)
                .Effective(DateTimeOffset.Now.AddDays(60))
                .Link("policy.html")
                .Title("Version Sunset Policy")
                .Type("text/html");
        })
    .AddMvc()
    .AddApiExplorer(
        options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        })
    .AddOpenApi(options => options.Document.AddScalarTransformers());

builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

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
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();
#endregion

#region Root redirect
app.MapGet("/", () => Results.Redirect("/scalar"));
#endregion

#region OpenAPI (FIXED VERSION)

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.MapOpenApi().WithDocumentPerVersion();

    // Single merged document: shared endpoints shown once; version-specific ones tagged v1/v2
    app.MapGet("/openapi/all.json", async (HttpContext ctx) =>
    {
        var sp = ctx.RequestServices;
        var v1Doc = await sp.GetRequiredKeyedService<IOpenApiDocumentProvider>("v1")
            .GetOpenApiDocumentAsync(ctx.RequestAborted);
        var v2Doc = await sp.GetRequiredKeyedService<IOpenApiDocumentProvider>("v2")
            .GetOpenApiDocumentAsync(ctx.RequestAborted);

        foreach (var (v2Path, v2PathItem) in v2Doc.Paths.ToList())
        {
            var v1EquivPath = v2Path.Replace("/v2/", "/v1/");

            if (!v1Doc.Paths.TryGetValue(v1EquivPath, out var v1PathItem))
            {
                // Path only exists in v2 — add it wholesale
                foreach (var op in v2PathItem.Operations!.Values)
                    Tag(op, v1Doc, "v2");
                v1Doc.Paths[v2Path] = v2PathItem;
                continue;
            }

            foreach (var (method, v2Op) in v2PathItem.Operations!)
            {
                if (v1PathItem!.Operations!.TryGetValue(method, out var v1Op)
                    && v1Op.OperationId != v2Op.OperationId)
                {
                    // Same path+method, different action — version-specific pair
                    Tag(v1Op, v1Doc, "v1");
                    Tag(v2Op, v1Doc, "v2");
                    var item = new OpenApiPathItem();
                    item.Operations!.Add(method, v2Op);
                    v1Doc.Paths[v2Path] = item;
                }
            }
        }

        return Results.Content(
            await v1Doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0, ctx.RequestAborted),
            "application/json");

        static void Tag(OpenApiOperation op, OpenApiDocument doc, string version)
        {
            doc.Tags ??= new HashSet<OpenApiTag>();
            if (doc.Tags.All(t => t.Name != version))
                doc.Tags.Add(new OpenApiTag { Name = version });

            // Replace tags so this endpoint has its own sidebar section
            op.Tags = new HashSet<OpenApiTagReference> { new OpenApiTagReference(version, doc, null!) };

            op.Summary = op.Summary is null ? $"[{version}]" : $"[{version}] {op.Summary}";
        }
    });

    app.MapScalarApiReference(options =>
        options.AddDocument("all", "Movie API", "/openapi/all.json"));

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
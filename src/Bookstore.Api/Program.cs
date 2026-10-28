using Bookstore.Api.Infrastructure;
using Bookstore.Application.DependencyInjection;
using Bookstore.Infrastructure.DependencyInjection;
using Bookstore.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Bookstore API (.NET 10)";
        document.Info.Version = "v1";
        document.Info.Description = "Clean Architecture Bookstore task with OAuth2 client-credentials and implicit security flows.";

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        var clientCredentialsScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "OAuth2 Client Credentials Flow for Machine-to-Machine CRUD operations.",
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("/api/auth/connect/token", UriKind.Relative),
                    Scopes = new Dictionary<string, string> { { "books.crud", "Full access to create, update, and delete books" } }
                }
            }
        };

        var implicitScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "OAuth2 Implicit Flow for User-Facing Search authorization.",
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("/api/auth/authorize", UriKind.Relative),
                    Scopes = new Dictionary<string, string> { { "books.search", "Access to search functionality" } }
                }
            }
        };

        document.Components.SecuritySchemes["ClientCredentialsAuth"] = clientCredentialsScheme;
        document.Components.SecuritySchemes["ImplicitAuth"] = implicitScheme;

        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("ClientCredentialsAuth", document)] = []
            },
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("ImplicitAuth", document)] = []
            }
        ];

        return Task.CompletedTask;
    });
});

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCrudScope", policy =>
    policy.RequireClaim("scope", "books.crud"));

    options.AddPolicy("RequireSearchScope", policy =>
    policy.RequireClaim("scope", "books.search"));
});

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

await app.Services.ApplyMigrationsAndSeedAsync();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bookstore API Engine v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

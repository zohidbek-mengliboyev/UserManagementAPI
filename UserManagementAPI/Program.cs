using FastEndpoints;
using FastEndpoints.Swagger;
using Npgsql;
using System.Data;
using UserManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "User Management API";
        s.Version = "v1";
    };
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

app.UseFastEndpoints();

app.UseSwaggerGen();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

app.Run();

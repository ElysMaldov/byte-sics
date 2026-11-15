using CachingAuth;
using CachingAuth.Repositories;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());

// Add services to the container.

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// db
var dbFileName = builder.Configuration.GetConnectionString("Sqlite") ?? throw new InvalidOperationException("Sqlite connection string not found");
var dbPath = Path.Combine(AppContext.BaseDirectory, dbFileName);
builder.Services.AddScoped<ICustomerRepository>(provider => new CustomerRepository(dbPath, provider.GetRequiredService<IMemoryCache>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();

app.UseAuthorization();

await app.InitializeDatabaseAsync();

app.MapControllers();

app.Run();

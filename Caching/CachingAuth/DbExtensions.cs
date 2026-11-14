using CachingAuth.Repositories;

namespace CachingAuth;

public static class DbExtensions
{
  public static async Task InitializeDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

    await repo.InitializeDatabaseAsync();
  }
}
using System;
using System.Data;
using CachingAuth.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;

namespace CachingAuth.Repositories;

public interface ICustomerRepository
{
  Task InitializeDatabaseAsync();

  Task<Guid> CreateAsync(Customer customer, CancellationToken ct = default);

  Task<IEnumerable<Customer>> ListAsync(CancellationToken ct = default);
  Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<bool> UpdateNameAsync(Guid id, (string FirstName, string LastName) names, CancellationToken ct = default);

  Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class CustomerRepository : ICustomerRepository
{
  private readonly string _connectionString;
  private readonly IMemoryCache _cache;

  public CustomerRepository(string dbPath, IMemoryCache cache)
  {
    _connectionString = $"Data Source={dbPath}";
    _cache = cache;
  }

  private IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
  private static MemoryCacheEntryOptions CacheOptions =>
    new MemoryCacheEntryOptions()
        // .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
        .RegisterPostEvictionCallback(OnPostEviction);

  public async Task InitializeDatabaseAsync()
  {
    string sql = $"""
      CREATE TABLE IF NOT EXISTS {Customer.TableName} (
          Id TEXT PRIMARY KEY NOT NULL,
          FirstName TEXT NOT NULL,
          LastName TEXT NOT NULL,
          Email TEXT NOT NULL
      );
    """;

    using var connection = CreateConnection();
    await connection.ExecuteAsync(sql);
  }

  public async Task<Guid> CreateAsync(Customer customer, CancellationToken ct = default)
  {
    string sql = $"""
      INSERT INTO {Customer.TableName} (Id, FirstName, LastName, Email)
      VALUES (@Id, @FirstName, @LastName, @Email);
    """;

    using var connection = CreateConnection();
    await connection.ExecuteScalarAsync<string>(new(sql, customer, cancellationToken: ct));

    _cache.Set(GetCustomerKey(customer.Id), customer, CacheOptions);

    return customer.Id; ;
  }

  public async Task<IEnumerable<Customer>> ListAsync(CancellationToken ct = default)
  {
    const string sql = $"""SELECT Id, FirstName, LastName, Email FROM {Customer.TableName}""";

    using var connection = CreateConnection();
    return await connection.QueryAsync<Customer>(new(sql, cancellationToken: ct));
  }

  public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
  {
    const string sql = $"""SELECT Id, FirstName, LastName, Email FROM {Customer.TableName} WHERE Id = @Id""";

    if (_cache.TryGetValue(GetCustomerKey(id), out var cached) && cached is Customer cachedCustomer)
    {
      Console.WriteLine($"{cachedCustomer.Id} acquired from cache");
      return cachedCustomer;
    }

    using var connection = CreateConnection();
    var customer = await connection.QuerySingleOrDefaultAsync<Customer?>(sql, new { Id = id });

    if (customer != null)
    {
      _cache.Set(GetCustomerKey(customer.Id), customer, CacheOptions);
      Console.WriteLine($"{id} saved to cache");
    }

    return customer;
  }

  // public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
  // {
  //   return await _cache.GetOrCreateAsync(GetCustomerKey(id), async entry =>
  //   {
  //     entry.SetOptions(CacheOptions);

  //     const string sql = $"""SELECT Id, FirstName, LastName, Email FROM {Customer.TableName} WHERE Id = @Id""";

  //     using var connection = CreateConnection();
  //     var customer = await connection.QuerySingleOrDefaultAsync<Customer?>(
  //         new(sql, new { Id = id }, cancellationToken: ct));
  //     Console.WriteLine($"{id} saved to cache");

  //     return customer;
  //   });
  // }

  public async Task<bool> UpdateNameAsync(Guid id, (string FirstName, string LastName) names, CancellationToken ct = default)
  {
    const string sql = $"""
      UPDATE {Customer.TableName}
      SET FirstName = @FirstName, LastName = @LastName
      WHERE Id = @Id
    """;

    using var connection = CreateConnection();
    int rowsAffected = await connection.ExecuteAsync(
      new(sql, new { Id = id, names.FirstName, names.LastName }, cancellationToken: ct)
    );

    _cache.Remove(GetCustomerKey(id));

    return rowsAffected > 0;
  }

  public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
  {
    const string sql = $"""DELETE FROM {Customer.TableName} WHERE Id = @Id""";

    using var connection = CreateConnection();
    int rowsAffected = await connection.ExecuteAsync(new(sql, new { Id = id }, cancellationToken: ct));

    _cache.Remove(GetCustomerKey(id));

    return rowsAffected > 0;
  }

  private static void OnPostEviction(object key, object? customerObj, EvictionReason reason, object? state)
  {
    if (customerObj is Customer customer)
    {
      Console.WriteLine($"[Customer:{key}] {customer.FirstName} {customer.LastName} evicted: {reason}");
    }
  }

  private static (Type CustomerType, string Id) GetCustomerKey(Guid customerId)
    => (typeof(Customer), customerId.ToString());
}

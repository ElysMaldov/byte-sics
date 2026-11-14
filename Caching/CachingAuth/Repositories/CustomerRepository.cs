using System;
using System.Data;
using CachingAuth.Models;
using Dapper;
using Microsoft.Data.Sqlite;

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

  public CustomerRepository(string dbPath)
  {
    _connectionString = $"Data Source={dbPath}";
  }

  private IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

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
    await connection.ExecuteScalarAsync<string>(sql, customer);

    return customer.Id; ;
  }

  public async Task<IEnumerable<Customer>> ListAsync(CancellationToken ct = default)
  {
    const string sql = $"""SELECT Id, FirstName, LastName, Email FROM {Customer.TableName}""";

    using var connection = CreateConnection();
    return await connection.QueryAsync<Customer>(sql);
  }

  public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
  {
    const string sql = $"""SELECT Id, FirstName, LastName, Email FROM {Customer.TableName} WHERE Id = @Id""";

    using var connection = CreateConnection();
    return await connection.QuerySingleOrDefaultAsync<Customer?>(sql, new { Id = id });
  }

  public async Task<bool> UpdateNameAsync(Guid id, (string FirstName, string LastName) names, CancellationToken ct = default)
  {
    const string sql = $"""
      UPDATE {Customer.TableName}
      SET FirstName = @FirstName, LastName = @LastName
      WHERE Id = @Id
    """;

    using var connection = CreateConnection();
    int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, names.FirstName, names.LastName });

    return rowsAffected > 0;
  }

  public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
  {
    const string sql = $"""DELETE FROM {Customer.TableName} WHERE Id = @Id""";

    using var connection = CreateConnection();
    int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

    return rowsAffected > 0;
  }
}

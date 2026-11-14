namespace CachingAuth.Models;

public sealed record Customer
{
  public const string TableName = "customers";

  public Guid Id { get; init; } = Guid.CreateVersion7();
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
}
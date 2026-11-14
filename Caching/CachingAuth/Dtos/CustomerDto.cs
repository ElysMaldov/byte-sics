using System.ComponentModel;

namespace CachingAuth.Dtos;

public sealed record CustomerDto
{
  public required Guid Id { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
}

public sealed record CreateCustomerDto
{
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
}


public sealed record UpdateCustomerDto
{
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
}




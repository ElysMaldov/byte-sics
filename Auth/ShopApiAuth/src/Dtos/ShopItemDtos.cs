namespace ShopApiAuth.src.Dtos;



public sealed record NewItemDto
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
}

public sealed record ItemDto
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required decimal PriceAmount { get; init; }
  public required string CurrencyCode { get; init; }
  public required int Quantity { get; init; }
  public required DateTime CreatedAt { get; init; }
  public DateTime? LastUpdate { get; init; }
}

public sealed record CreateItemDto
{
  public required string Name { get; init; }
  public required decimal PriceAmount { get; init; }
  public required string CurrencyCode { get; init; }
  public required int Quantity { get; init; }
}

public sealed record UpdateItemDto
{
  public required string Name { get; init; }
  public required decimal PriceAmount { get; init; }
  public required int Quantity { get; init; }

}

public sealed record DeleteItemDto
{
  public required Guid Id { get; init; }
}
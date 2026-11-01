namespace ShopApiAuth.src.Entities;

public sealed record ShopItem
{
  public required Guid Id { get; init; } = Guid.NewGuid();
  public required string Name { get; init; }
  public required Price Price { get; init; }
  public int Quantity { get; init; }
  public required DateTime CreatedAt { get; init; } = DateTime.Now;
  public DateTime? LastUpdate { get; init; }
}

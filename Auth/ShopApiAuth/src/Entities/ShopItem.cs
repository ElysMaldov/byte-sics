using Bogus;

namespace ShopApiAuth.src.Entities;

public sealed record ShopItem
{
  public Guid Id { get; init; } = Guid.NewGuid();
  public required string Name { get; init; }
  public required Price Price { get; init; }
  public int Quantity { get; init; }
  public required DateTime CreatedAt { get; init; } = DateTime.Now;
  public DateTime? LastUpdate { get; init; }

  public static List<ShopItem> Generate(int count = 5)
  {
    var shopItemFaker = new Faker<ShopItem>()
          .RuleFor(i => i.Id, f => f.Random.Guid())
          .RuleFor(i => i.Name, f => f.Commerce.ProductName())
          .RuleFor(i => i.Price, f => new Price()
          {
            Amount = f.Random.Decimal(0m, 1000m),
            CurrencyCode = f.Random.String(3)
          })
          .RuleFor(i => i.Quantity, f => f.Random.Int(0, 1_000))
          .RuleFor(i => i.CreatedAt, f => f.Date.Future())
          .RuleFor(i => i.LastUpdate, f => f.Date.Future().OrNull(f, 0.5f));

    return shopItemFaker.Generate(count);
  }
}

public static class ShopItems
{
  public static readonly List<ShopItem> items = ShopItem.Generate();
}
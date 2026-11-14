namespace ShopApiAuth.src.Entities;

public sealed record Price
{
  public required decimal Amount { get; init; }
  public required string CurrencyCode { get; init; }
}

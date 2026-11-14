using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ShopApiAuth.src.Dtos;
using ShopApiAuth.src.Entities;

namespace ShopApiAuth.src.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly List<ShopItem> _items = ShopItems.items;

    // TODO add auth

    [HttpPost]
    [Route("products")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<NewItemDto> AddItem([FromBody] CreateItemDto dto)
    {
        // Should add mapping and validation, but no time for that right now

        ShopItem newItem = new()
        {
            CreatedAt = DateTime.Now,
            Name = dto.Name,
            Price = new()
            {
                Amount = dto.PriceAmount,
                CurrencyCode = dto.CurrencyCode
            }
        };

        _items.Add(newItem);

        return Created();
    }

    [HttpGet]
    [Route("products")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IReadOnlyList<ItemDto>> GetItems()
    {
        var dtos = _items.Select(item => new ItemDto()
        {
            CreatedAt = item.CreatedAt,
            CurrencyCode = item.Price.CurrencyCode,
            Id = item.Id,
            PriceAmount = item.Price.Amount,
            Name = item.Name,
            Quantity = item.Quantity
        }).ToList();

        return dtos;
    }

    [HttpGet]
    [Route("products/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ItemDto> GetItemById(string id)
    {
        var item = FindItemById(new Guid(id));

        if (item == null) return NotFound();

        return new ItemDto()
        {
            CreatedAt = item.CreatedAt,
            CurrencyCode = item.Price.CurrencyCode,
            Id = item.Id,
            PriceAmount = item.Price.Amount,
            Name = item.Name,
            Quantity = item.Quantity
        };
    }

    [HttpPut]
    [Route("products/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateItem(string id, [FromBody] UpdateItemDto dto)
    {
        var item = FindItemById(new Guid(id));

        if (item == null) return NotFound();

        int index = _items.FindIndex(i => i == item);

        if (index >= 0)
        {
            _items[index] = item with
            {

                LastUpdate = DateTime.Now,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Price = item.Price with { Amount = dto.PriceAmount }
            };
        }

        return NoContent();
    }

    [HttpDelete]
    [Route("products/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteItemById(string id)
    {
        var item = FindItemById(new Guid(id));

        if (item == null) return NotFound();

        int index = _items.FindIndex(i => i == item);

        if (index >= 0)
        {
            _items.RemoveAt(index);
        }

        return NoContent();
    }

    private ShopItem? FindItemById(Guid id)
    {
        var item = _items.Find(i => i.Id.Equals(id));

        return item;
    }
}


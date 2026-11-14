using CachingAuth.Dtos;
using CachingAuth.Models;
using CachingAuth.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CachingAuth.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerApiController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCustomer([FromBody] CreateCustomerDto dto, CancellationToken ct)
    {
        Customer newCustomer = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        return await _customerRepository.CreateAsync(newCustomer, ct);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> ListCustomer(CancellationToken ct)
    {
        var customers = await _customerRepository.ListAsync(ct);

        return customers.Select(c => new CustomerDto()
        {
            Email = c.Email,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Id = c.Id
        }).ToList();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CustomerDto?>> GetCustomerById(string id, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByIdAsync(Guid.Parse(id), ct);

        if (customer == null) return NotFound();

        return new CustomerDto()
        {
            Id = customer.Id,
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
        };
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<ActionResult<bool>> UpdateName(string id, [FromBody] UpdateCustomerDto dto, CancellationToken ct)
    {
        var isUpdated = await _customerRepository.UpdateNameAsync(Guid.Parse(id), (dto.FirstName, dto.LastName), ct);

        return isUpdated;
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken ct)
    {
        var isDeleted = await _customerRepository.DeleteAsync(Guid.Parse(id), ct);

        if (!isDeleted) return NotFound();

        return isDeleted;
    }
}


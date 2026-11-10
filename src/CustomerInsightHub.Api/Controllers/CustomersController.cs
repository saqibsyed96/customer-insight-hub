using CustomerInsightHub.Application.Customers;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsightHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerRepository customers) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await customers.GetAllAsync(cancellationToken));
    }
}

using CustomerInsightHub.Application.Common;
using CustomerInsightHub.Application.Customers;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsightHub.Api.Controllers;

/// <summary>
/// HTTP only: reads the request, calls the service, and turns the service's answer into a
/// status code. No business rules live here.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService customers) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await customers.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await customers.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : MapFailure(result);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(SaveCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await customers.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : MapFailure(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, SaveCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await customers.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : MapFailure(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await customers.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : MapFailure(result);
    }

    // Service outcome -> HTTP status: Invalid = 400, NotFound = 404, Conflict = 409.
    private ActionResult MapFailure(ServiceResult result) => result.Status switch
    {
        ResultStatus.Invalid => ValidationProblem(new ValidationProblemDetails(
            result.Errors.ToDictionary(e => e.Key, e => e.Value))),
        ResultStatus.NotFound => NotFound(),
        ResultStatus.Conflict => Conflict(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Detail = result.Message
        }),
        _ => StatusCode(StatusCodes.Status500InternalServerError)
    };
}

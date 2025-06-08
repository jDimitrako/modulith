using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Modulith.NewModule.Application.Todo;
using Modulith.NewModule.Api.OpenTelemetry;

namespace Modulith.NewModule.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("api")] // Default API rate limiting
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly OpenTelemetryExample _openTelemetryExample;

    public TodoController(IMediator mediator, OpenTelemetryExample openTelemetryExample)
    {
        _mediator = mediator;
        _openTelemetryExample = openTelemetryExample;
    }

    [HttpGet]
    [EnableRateLimiting("burst")] // Burst rate limiting for list endpoint
    public async Task<IActionResult> GetTodos()
    {
        _openTelemetryExample.CalculatePrice("example-product-1", 10);
        var result = await _mediator.Send(new GetTodosQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("concurrent")] // Concurrent rate limiting for single item
    public async Task<IActionResult> GetTodo(int id)
    {
        _openTelemetryExample.ProcessOrder($"order-{id}", "example-product-2");
        var result = await _mediator.Send(new GetTodoQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [EnableRateLimiting("auth")] // Stricter rate limiting for creation
    public async Task<IActionResult> CreateTodo([FromBody] AddTodoCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTodo), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("chunked")] // Chunked rate limiting for updates
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("auth")] // Stricter rate limiting for deletion
    public async Task<IActionResult> DeleteTodo(int id)
    {
        await _mediator.Send(new DeleteTodoCommand(id));
        return NoContent();
    }
} 
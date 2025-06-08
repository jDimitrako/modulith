using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Modulith.NewModule.Application.Todo;

namespace Modulith.NewModule.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("api")] // Default API rate limiting
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [EnableRateLimiting("burst")] // Burst rate limiting for list endpoint
    public async Task<IActionResult> GetTodos()
    {
        var result = await _mediator.Send(new GetTodosQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("concurrent")] // Concurrent rate limiting for single item
    public async Task<IActionResult> GetTodo(int id)
    {
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
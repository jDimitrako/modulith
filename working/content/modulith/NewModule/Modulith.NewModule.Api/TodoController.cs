using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modulith.NewModule.Application.Commands;
using Modulith.NewModule.Application.Queries;
using Modulith.NewModule.Entities;

namespace Modulith.NewModule.Api;

[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;
    public TodoController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetTodos()
        => await _mediator.Send(new GetTodosQuery());

    [HttpPost]
    public async Task<ActionResult<TodoItem>> AddTodo([FromBody] AddTodoCommand command)
        => await _mediator.Send(command);
} 
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modulith.NewModule.Application.Commands;
using Modulith.NewModule.Application.Queries;
using Modulith.NewModule.Entities;
using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Api.Mappers;

namespace Modulith.NewModule.Api;

[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;
    public TodoController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<TodoItemDto>>> GetTodos()
    {
        var todos = await _mediator.Send(new GetTodosQuery());
        return todos.Select(TodoItemMapper.ToDto).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> AddTodo([FromBody] AddTodoCommand command)
    {
        var todo = await _mediator.Send(command);
        return TodoItemMapper.ToDto(todo);
    }
} 
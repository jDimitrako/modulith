using Microsoft.AspNetCore.Mvc;
using Modulith.NewModule.Application.Todos.Commands;
using Modulith.NewModule.Application.Todos.Queries;

namespace Modulith.NewModule.Api.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/todos")]
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public TodoController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodos([FromQuery] bool includeCompleted = true)
    {
        var query = new GetTodosQuery { IncludeCompleted = includeCompleted };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> AddTodo(AddTodoCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTodos), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoById(Guid id)
    {
        var query = new GetTodoByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
} 
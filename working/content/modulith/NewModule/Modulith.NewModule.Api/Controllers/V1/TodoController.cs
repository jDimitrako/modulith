using Microsoft.AspNetCore.Mvc;
using Modulith.NewModule.Application.Todos.Commands;
using Modulith.NewModule.Application.Todos.Queries;

namespace Modulith.NewModule.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
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
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodos()
    {
        var query = new GetTodosQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> AddTodo(AddTodoCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTodos), new { id = result.Id }, result);
    }
} 
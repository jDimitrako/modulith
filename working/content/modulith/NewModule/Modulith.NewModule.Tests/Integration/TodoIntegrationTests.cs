using FluentAssertions;
using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Application.Commands;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Modulith.NewModule.Tests.Integration;

public class TodoIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TodoIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.InitializeAsync().Wait(); // Ensure containers are started
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_Todo_Should_Create_And_Return_TodoItemDto()
    {
        // Arrange
        var command = new AddTodoCommand("Integration Test Todo");
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/todos", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todoDto = JsonConvert.DeserializeObject<TodoItemDto>(await response.Content.ReadAsStringAsync());
        todoDto.Should().NotBeNull();
        todoDto.Title.Should().Be("Integration Test Todo");
        todoDto.IsComplete.Should().BeFalse();

        // Further assertions to check database (e.g., via a direct query or another API call)
        var getResponse = await _client.GetAsync("/api/todos");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = JsonConvert.DeserializeObject<List<TodoItemDto>>(await getResponse.Content.ReadAsStringAsync());
        todos.Should().ContainSingle(t => t.Title == "Integration Test Todo");
    }
} 
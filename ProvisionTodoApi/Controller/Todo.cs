using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProvisionTodoApi.Domain;
using TodoApi.Infrastructure;

namespace ProvisionTodoApi.Controller;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController()
    {
        var options = new DbContextOptionsBuilder<TodoContext>().UseSqlite("Data Source=Todo.db");
        _context = new TodoContext(options.Options);
    }

    [HttpGet]
    public IActionResult GetTodos()
    {
        var todos = _context.Todos.ToList();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public IActionResult GetTodoById(int id)
    {
        var todo = _context.Todos.Find(id);
        if (todo == null)
            return NotFound();

        return Ok(todo);
    }

    [HttpPost]
    public IActionResult AddTodo([FromBody] string description)
    {
        var todo = new Todo
        {
            Description = description,
            IsComplete = false,
            Priority = description.Contains("urgent", StringComparison.OrdinalIgnoreCase) ? "High" : "Normal"
        };
        _context.Todos.Add(todo);
        _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTodoStatus(int id, [FromBody] bool isComplete)
    {
        var todo = _context.Todos.Find(id);
        if (todo == null)
            return NotFound();

        todo.IsComplete = isComplete;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTodoById(int id)
    {
        var todo = _context.Todos.Find(id);
        if (todo == null)
            return NotFound();

        _context.Todos.Remove(todo);
        _context.SaveChanges();

        return NoContent();
    }
}
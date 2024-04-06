using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Application;
using TodoList.Data.Domain;

namespace TodoList.Controllers;
public class TaskController : Controller
{
    private readonly TodoListDataContext _context;
    private readonly ILogger<UserController> _logger;

    public TaskController(TodoListDataContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var users = _context.Tasks.ToList();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(TodoListTask task)
    {
        if (ModelState.IsValid)
        {
            try
            {
                task.DueDate = DateTime.Now.ToUniversalTime();
                _context.Tasks.Add(task);
                _context.SaveChanges();
                _logger.Log(LogLevel.Information, "Model is valid, user added");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is Npgsql.PostgresException postgresException && postgresException.SqlState == "23505")
                {
                    ModelState.AddModelError("Email", "Email address is already in use.");
                }
                else
                {
                    _logger.LogError($"Error saving user: {ex.Message}");
                }
            }
        }

        return View();
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var task = _context.Tasks.Find(id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpPost]
    public IActionResult Edit(int id, TodoListTask task)
    {
        if (id != task.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(task);
                _context.SaveChanges();
                _logger.Log(LogLevel.Information, "Task updated successfully");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Error updating task: {ex.Message}");
            }
        }

        return View(task);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var task = _context.Tasks.Find(id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(TodoListTask task)
    {
        _context.Tasks.Remove(task);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}

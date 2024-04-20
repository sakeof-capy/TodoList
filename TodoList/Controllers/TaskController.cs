using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoList.Data.Application;
using TodoList.Data.Domain;
using TodoList.Models;

namespace TodoList.Controllers;

[Authorize]
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
    public async Task<IActionResult> Index()
    {
        var currentUserClaims = HttpContext.User.Claims;
        string? userEmail = currentUserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("Email claim not found in JWT token.");
        }

        var tasks = await _context.Tasks.Where(t => t.OwnerEmail == userEmail).ToListAsync();

        return View(tasks);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel inputTask)
    {
        if (ModelState.IsValid)
        {
            var currentUserClaims = HttpContext.User.Claims;
            string? userEmail = currentUserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Email claim not found in JWT token.");
            }

            var task = new TodoListTask
            {
                OwnerEmail = userEmail,
                Title = inputTask.Title,
                Description = inputTask.Description,
                DueDate = DateTime.Now.ToUniversalTime(),
                IsComplete = inputTask.IsComplete,
            };

            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
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
    public async Task<IActionResult> Edit(int id)
    {
        var dbTask = await _context.Tasks.FindAsync(id);

        if (dbTask == null)
        {
            return NotFound();
        }

        var task = new TaskViewModel
        {
            Title = dbTask.Title,
            Description = dbTask.Description,
            IsComplete = dbTask.IsComplete,
        };

        return View(task);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TaskViewModel inputTask)
    {
        if (ModelState.IsValid)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            task.Title = inputTask.Title;
            task.Description = inputTask.Description;
            task.IsComplete = inputTask.IsComplete;

            try
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Error updating task: {ex.Message}");
            }
        }

        return View(inputTask);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(TodoListTask task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

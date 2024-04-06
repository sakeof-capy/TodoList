using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoList.Data.Application;
using TodoList.Data.Domain;

namespace TodoList.Controllers;

public class UserController : Controller
{
    private readonly TodoListDataContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(TodoListDataContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var users = _context.Users.ToList();
        _logger.Log(LogLevel.Information, "Index called");
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(TodoListUser user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Users.Add(user);
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

        return View(user);
    }

    [HttpGet]
    public IActionResult GetUser(string email)
    {
        var user = _context.Users.Find(email);
        return user != null ? Ok(user) : NotFound();
    }

    [HttpGet]
    public IActionResult Edit(string? email)
    {
        if (email == null)
        {
            return NotFound();
        }

        var user = _context.Users.Find(email);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public IActionResult Edit(string email, TodoListUser user)
    {
        if (email != user.Email)
        {
            _logger.Log(LogLevel.Information, "Error: Trying to edit a wrong user: id mismatch");
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(user);
            _context.SaveChanges();
            _logger.Log(LogLevel.Information, "Model is valid, user edited");
            return RedirectToAction(nameof(Index));
        }
        else
        {
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    _logger.LogError($"ModelState Error: {error.ErrorMessage}");
                }
            }

            _logger.LogError("Invalid model: " + JsonConvert.SerializeObject(user));

            return View(user);
        }
    }

    [HttpGet]
    public IActionResult Delete(string? email)
    {
        if (email == null)
        {
            return NotFound();
        }

        var user = _context.Users.Find(email);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }


    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(TodoListUser user)
    {
        _context.Users.Remove(user);
        _context.SaveChanges();
        _logger.Log(LogLevel.Information, "Deleted");
        return RedirectToAction(nameof(Index));
    }
}

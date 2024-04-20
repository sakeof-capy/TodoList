using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoList.Data.Application;
using TodoList.Data.Domain;
using TodoList.Data.JWT;
using TodoList.Models;

namespace TodoList.Controllers;

[Authorize]
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
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        if (user == null)
        {
            return RedirectToAction(nameof(Index)); // TODO: notify about the error login
        }

        var token = JWTManager.GenerateJwtToken(user);

        Response.Cookies.Append(JWTManager.TOKEN_COOKIES_KEY, token, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddHours(JWTManager.TOKEN_EXPIRATION_HOURS)
        });

        return RedirectToAction("Index", "Task");
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserViewModel inputUser)
    {
        if (ModelState.IsValid)
        {
            var user = new TodoListUser
            {
                Email = inputUser.Email,
                Password = inputUser.Password,
            };

            try
            {
                _context.Users.Add(user);
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

        return View(inputUser);
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _context.Users.FindAsync(email);
        return user != null ? Ok(user) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? email)
    {
        if (email == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string email, UserViewModel inputUser)
    {
        if (email != inputUser.Email)
        {
            _logger.LogError("Error: Trying to edit a wrong user: id mismatch");
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var user = new TodoListUser
            {
                Email = inputUser.Email,
                Password = inputUser.Password,
            };

            _context.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Model is valid, user edited");
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

            _logger.LogError("Invalid model: " + JsonConvert.SerializeObject(inputUser));

            return View(inputUser);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string? email)
    {
        if (email == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(email);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }


    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(UserViewModel inputUser)
    {
        var user = new TodoListUser
        {
            Email = inputUser.Email,
            Password = inputUser.Password,
        };

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        _logger.Log(LogLevel.Information, "Deleted");

        return RedirectToAction(nameof(Index));
    }
}

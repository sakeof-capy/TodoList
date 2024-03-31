using System.ComponentModel.DataAnnotations;

namespace TodoList.Data.Domain;

public class TodoListUser
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is a required field of TodoListUser")]
    [StringLength(320, ErrorMessage = "Email cannot be more than 320 characters long.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is a required field of TodoListUser")]
    [StringLength(20, ErrorMessage = "Password cannot be more than 20 characters long.")]
    public string? Password { get; set; }
}
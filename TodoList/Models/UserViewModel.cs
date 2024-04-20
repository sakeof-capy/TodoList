using System.ComponentModel.DataAnnotations;

namespace TodoList.Models;

public class UserViewModel
{
    [Key]
    [Required(ErrorMessage = "Email is a required field of TodoListUser")]
    [StringLength(320, ErrorMessage = "Email cannot be more than 320 characters long.")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is a required field of TodoListUser")]
    [StringLength(20, ErrorMessage = "Password cannot be more than 20 characters long.")]
    public string? Password { get; set; }
}

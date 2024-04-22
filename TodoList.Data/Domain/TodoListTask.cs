using System.ComponentModel.DataAnnotations;

namespace TodoList.Data.Domain;

public class TodoListTask
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is a required field of TodoListTask")]
    [StringLength(320, ErrorMessage = "Email cannot be more than 320 characters long.")]
    [EmailAddress]
    public string? OwnerEmail { get; set; }

    [Required(ErrorMessage = "Title is required field of TodoListTask")]
    [StringLength(50, ErrorMessage = "Title cannot be more than 50 characters long.")]
    public string? Title { get; set; }


    [StringLength(50, ErrorMessage = "Description cannot be more than 50 characters long.")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; } = null;
    public bool IsComplete { get; set; } = false;
}
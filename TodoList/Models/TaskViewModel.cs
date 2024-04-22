using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Models;

public class TaskViewModel
{
    [Required(ErrorMessage = "Title is required field of TodoListTask")]
    [StringLength(50, ErrorMessage = "Title cannot be more than 50 characters long.")]
    public string Title { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be more than 500 characters long.")]
    public string? Description { get; set; }

    public bool IsComplete { get; set; }

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }
}

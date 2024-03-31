using Microsoft.EntityFrameworkCore;
using TodoList.Data.Domain;

namespace TodoList.Data.Application;

public class TodoListDataContext : DbContext
{
    public TodoListDataContext(DbContextOptions<TodoListDataContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }

    public DbSet<TodoListUser> Users { get; set; }
    public DbSet<TodoListTask> Tasks { get; set; }
}

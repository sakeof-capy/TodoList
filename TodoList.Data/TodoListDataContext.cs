using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Data;

public class TodoListDataContext : DbContext
{
    public TodoListDataContext(DbContextOptions<TodoListDataContext> options)
        : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }

    public DbSet<TodoListUser> Users { get; set; }
}

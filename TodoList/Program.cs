using Microsoft.EntityFrameworkCore;
using TodoList.Data.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TodoListDataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TodoListDb"),
    b => b.MigrationsAssembly("TodoList.Data"))
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

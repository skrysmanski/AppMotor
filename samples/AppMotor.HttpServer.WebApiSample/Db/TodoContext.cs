using AppMotor.HttpServer.WebApiSample.Models;

using Microsoft.EntityFrameworkCore;

namespace AppMotor.HttpServer.WebApiSample.Db;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}

using AppMotor.HttpServer.WebApiSample.Models;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace AppMotor.HttpServer.WebApiSample.Db;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}

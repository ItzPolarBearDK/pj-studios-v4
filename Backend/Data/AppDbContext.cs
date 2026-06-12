using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}

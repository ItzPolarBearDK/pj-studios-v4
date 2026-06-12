using Backend.Api.Data;
using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/todos", async (AppDbContext dbContext) =>
    await dbContext.TodoItems
        .OrderByDescending(item => item.CreatedAtUtc)
        .ToListAsync());

app.MapPost("/api/todos", async (AppDbContext dbContext, TodoItem todo) =>
{
    if (string.IsNullOrWhiteSpace(todo.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var todoItem = new TodoItem
    {
        Title = todo.Title.Trim(),
        IsDone = todo.IsDone
    };

    dbContext.TodoItems.Add(todoItem);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/api/todos/{todoItem.Id}", todoItem);
});

app.MapPut("/api/todos/{id:int}/toggle", async (AppDbContext dbContext, int id) =>
{
    var todo = await dbContext.TodoItems.FindAsync(id);

    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.IsDone = !todo.IsDone;
    await dbContext.SaveChangesAsync();

    return Results.Ok(todo);
});

app.MapDelete("/api/todos/{id:int}", async (AppDbContext dbContext, int id) =>
{
    var todo = await dbContext.TodoItems.FindAsync(id);

    if (todo is null)
    {
        return Results.NotFound();
    }

    dbContext.TodoItems.Remove(todo);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

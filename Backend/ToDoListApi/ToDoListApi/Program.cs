using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Services;
using ToDoListApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IToDoListService, ToDoListService>();
builder.Services.AddSingleton<IToDoListRepository, InMemoryTodoRepository>(); // singleton for in-memory store

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Services;
using ToDoListApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IToDoListService, ToDoListService>();
builder.Services.AddSingleton<IToDoListRepository, InMemoryToDoRepository>();

// Add CORS policy for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Use the CORS policy
app.UseCors("LocalDev");

app.UseAuthorization();

app.MapControllers();

app.Run();

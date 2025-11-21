using System.Net;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Services;
using ToDoListApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IToDoListService, ToDoListService>();
builder.Services.AddSingleton<IToDoListRepository, InMemoryToDoRepository>(); // singleton for in-memory storage

// Add CORS policy for local development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("LocalDev", policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular default port
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "An unhandled exception occurred");

            var errorMessage = app.Environment.IsDevelopment()
                ? exception.Message
                : "An error occurred while processing your request.";

            var response = new { error = errorMessage };
            await context.Response.WriteAsJsonAsync(response);
        }
    });
});

app.UseHttpsRedirection();

// Use the CORS policy only in development
if (app.Environment.IsDevelopment())
{
    app.UseCors("LocalDev");
}

app.UseAuthorization();

app.MapControllers();

app.Run();

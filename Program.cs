using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5273");

var app = builder.Build();


List<string> messages = new List<string>();


app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    
    string response = "<h1>Сообщения:</h1><ul>";
    foreach (var message in messages)
    {
        response += $"<h3>{message}</h3>";
    }
    response += "</ul>";
    await context.Response.WriteAsync(response);
});

// POST /message
app.MapPost("/message", async (HttpContext context) =>
{
    using (var reader = new StreamReader(context.Request.Body))
    {
        string message = await reader.ReadToEndAsync();
        messages.Add(message);
        Console.WriteLine($"Получено сообщение: {message}");
        await context.Response.WriteAsync($"Сообщение получено: {message}");
    }
});

// GET /name
app.MapGet("/name", () => "MyServer");

app.Run();

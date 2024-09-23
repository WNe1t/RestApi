var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5273");

var app = builder.Build();


List<string> messages = new List<string>();

//MapGet() даёт нам понять какой маршрут нужно выполнить /message или /name
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8"; // text/html - это тип ответа сервера, utf-8 - кодировка для коректного отображения текста

    string response = "<h1>Сообщения:</h1><ul>";
    foreach (var message in messages)
    {
        response += $"<h3>{message}</h3>";
    }
    response += "</ul>";
    await context.Response.WriteAsync(response); // отправка готовой html страницы в ответ на запрос клиента 
});

// POST /message
app.MapPost("/message", async (HttpContext context) =>
{
    using (var reader = new StreamReader(context.Request.Body)) //StreamReader - читает данные из потока, context.Request.Body - поток данных клиента
    {
        string message = await reader.ReadToEndAsync(); //ReadToEndAsync() - используется для асинхронного четения всего содержимого потока до самого конца
        messages.Add(message);
        Console.WriteLine($"Получено сообщение: {message}");
        await context.Response.WriteAsync($"Сообщение получено: {message}"); //отправка ответа сервера к клиенту
    }
});

// GET /name
app.MapGet("/name", () => "MyServer"); // возвращает обычную строку

app.Run(); //запускает сервер

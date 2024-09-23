using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://192.168.1.109:5273");

        var app = builder.Build();

        // Включаем поддержку статических файлов
        app.UseStaticFiles(); // Позволяет серверу обслуживать статические файлы

        List<string> messages = new List<string>();

        // Отдаем index.html по корневому запросу
        app.MapGet("/", async context =>
        {
            context.Response.ContentType = "text/html; charset=utf-8"; // text/html - это тип ответа сервера, utf-8 - кодировка для коректного отображения текста
            await context.Response.SendFileAsync("Site/html/index.html"); // Отдаём index.html
        });

        // POST /message
        app.MapPost("/message", async (HttpContext context) =>
        {
            using (var reader = new StreamReader(context.Request.Body))
            {
                string json = await reader.ReadToEndAsync(); // ReadToEndAsync для прочтения JSON из тела запроса
                var messageModel = JsonSerializer.Deserialize<MessageModel>(json); // Десериализуем JSON в объект MessageModel

                if (messageModel != null && !string.IsNullOrEmpty(messageModel.message))
                {
                    messages.Add(messageModel.message); // Добавляем текст сообщения в список
                    Console.WriteLine($"Получено сообщение: {messageModel.message}");
                    await context.Response.WriteAsync($"Сообщение получено: {messageModel.message}"); 
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(":(");
                }
            }
        });

        // GET /name
        app.MapGet("/name", () => "MyServer");

        app.Run();
    }

    public class MessageModel
    {
        public string message { get; set; }
    }
}

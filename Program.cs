using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://172.29.13.124:5273");

        var app = builder.Build();

        List<string> messages = new List<string>();

        app.UseStaticFiles();

        // GET запрос для главной стр.
        app.MapGet("/", async context =>
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync("Site/html/index.html");
        });

        // Обработка POST запроса для получения сообщений
        app.MapPost("/message", async (HttpContext context) =>
        {
            using (var reader = new StreamReader(context.Request.Body))
            {
                string json = await reader.ReadToEndAsync();
                var messageModel = JsonSerializer.Deserialize<MessageModel>(json);

                if (messageModel != null && !string.IsNullOrEmpty(messageModel.message))
                {
                    // cохранение сообщений
                    messages.Add($"Кто то: {messageModel.message}");
                    Console.WriteLine($"Получено сообщение: {messageModel.message}");

                    //отправка сообщения на другой сервер
                    var sendingMessages = new SendingMessages();
                    var externalServerResponse = await sendingMessages.SendMessageToExternalServer(messageModel.message);

                    Console.WriteLine($"Собеседник: {externalServerResponse}");

                    await context.Response.WriteAsync("Сообщение отправлено.");
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Ошибка: сообщение пустое.");
                }
            }
        });

        // GET запрос для получения списка
        app.MapGet("/messages", () =>
        {
            return JsonSerializer.Serialize(messages);
        });

        // Запуск сервера
        app.Run();
    }

    public class MessageModel //JSON
    {
        public string message { get; set; }
    }
}

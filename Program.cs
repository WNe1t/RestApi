using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://172.29.13.124:5273");

        var app = builder.Build();

        List<string> messages = new List<string>();

        // Включаем поддержку статических файлов
        app.UseStaticFiles();

        // Отдаем index.html по корневому запросу
        app.MapGet("/", async context =>
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync("Site/html/index.html");
        });

        // POST /message
        app.MapPost("/message", async (HttpContext context) =>
        {
            using (var reader = new StreamReader(context.Request.Body))
            {
                string json = await reader.ReadToEndAsync();
                var messageModel = JsonSerializer.Deserialize<MessageModel>(json);

                if (messageModel != null && !string.IsNullOrEmpty(messageModel.message))
                {
                    messages.Add(messageModel.message);
                    Console.WriteLine($"Получено сообщение: {messageModel.message}");

                    // Отправляем сообщение на чужой сервер
                    await SendMessageToExternalServer(messageModel.message);

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

    // Тут отправка на другой сервер
    public static async Task SendMessageToExternalServer(string message)
    {
        using (var client = new HttpClient())
        {
            var baseUrl = "http://172.29.9.90:3400/message"; // путь / адресс

            
            Console.WriteLine($"Отправка сообщения на {baseUrl}: {message}");

            
            var messageModel = new { message = message }; // messageModel объект для отправки
            var json = JsonSerializer.Serialize(messageModel);

            
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // JSON в StringContent

            // POST запрос
            var response = await client.PostAsync(baseUrl, content);

            
            if (response.IsSuccessStatusCode) // Проверка запроса
            {
                Console.WriteLine($"Сообщение успешно отправлено на {baseUrl}");
            }
            else
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {response.StatusCode}");
            }
        }
    }
    public class MessageModel
    {
        public string message { get; set; }
    }
}
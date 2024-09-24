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

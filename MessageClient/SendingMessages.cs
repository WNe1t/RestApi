using System.Diagnostics;
using System.Text.Json;
using System.Text;

class SendingMessages
{
    // Отправка сообщения на другой сервер
    public async Task<string> SendMessageToExternalServer(string message)
    {
        using (var client = new HttpClient())
        {
            var baseUrl = "http://172.29.9.90:3400/message"; // URL внешнего сервера
            Console.WriteLine($"Отправка сообщения на {baseUrl}: {message}");

            var messageModel = new { message = message };
            var json = JsonSerializer.Serialize(messageModel);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var externalResponse = await response.Content.ReadAsStringAsync(); // Читаем ответ внешнего сервера
                Console.WriteLine($"Сообщение доставлено: {externalResponse}");
                return externalResponse; // Возвращаем ответ для отображения на сайте
            }
            else
            {
                Console.WriteLine($"Ошибка доставки сообщения: {response.StatusCode}");
                return $"Ошибка доставки сообщения: {response.StatusCode}";
            }
        }
    }

    // Повторная отправка сообщений
    public async Task<double> ResendingMessages(Func<Task> SendMessageToExternalServer)
    {
        int massageCount = 4; double Minuts;
        await Task.Delay(1); //задержка в 1 миллсек.
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < massageCount; i++)
        {
            await SendMessageToExternalServer();
        }
        stopwatch.Stop(); // Остановка таймера
        Minuts = stopwatch.Elapsed.TotalSeconds / 60; //stopwatch.Elapsed - получает общее затраченное время
        Console.WriteLine($"Отправленно {massageCount} в минутах: {Minuts}");
        return Minuts;
    }
}
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
            var baseUrl = "http://172.29.9.90:3400/message"; // Путь / адрес
            Console.WriteLine($"Отправка сообщения на {baseUrl}: {message}");

            var messageModel = new { message = message }; // messageModel объект для отправки
            var json = JsonSerializer.Serialize(messageModel);

            var content = new StringContent(json, Encoding.UTF8, "application/json"); // JSON в StringContent
            // POST запрос
            var response = await client.PostAsync(baseUrl, content);

            if (response.IsSuccessStatusCode) // Проверка запроса
            {
                Console.WriteLine($"Сообщение доставлено: {baseUrl}"); // URL при отправке
                //Console.WriteLine($"Сообщение номер {i}");
                return baseUrl;
            }
            else
            {
                Console.WriteLine($"Ошибка доставки сообщения: {response.StatusCode}");
                return response.StatusCode.ToString(); // Код ошибки как строка
            }

        }
    }

    public async Task<double> ResendingMessages(Func<Task> SendMessageToExternalServer)
    {
        int massageCount = 1000; var stopwatch = Stopwatch.StartNew(); double Minuts;
        await Task.Delay(1); //задержка в 1 миллсек.
        for(int i = 0; i < massageCount; i++)
        {
            await SendMessageToExternalServer();
        }
        stopwatch.Stop(); // Остановка таймера
        Minuts = stopwatch.Elapsed.TotalSeconds / 60;
        Console.WriteLine($"Отправленно {massageCount} в минутах: {Minuts}");
        return Minuts;
    }
}

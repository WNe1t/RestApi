using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using System.Threading.Tasks;

class MessageClientProgram
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://172.29.13.124:5273"; // URL сервера
        using var client = new HttpClient(); // HttpClient для отправки запросов

        while (true)
        {
            Console.Write("Сообщение (введите 'exit' для выхода): ");
            var message = Console.ReadLine();

            if (message?.ToLower() == "exit")
            {
                break;
            }

            // Создаем объект для отправки
            var messageModel = new { message = message };
            var json = JsonSerializer.Serialize(messageModel);


            // POST - запрос
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/message", content);

            if (response.IsSuccessStatusCode) // Проверяем успешность запроса
            {
                var responseMessage = await response.Content.ReadAsStringAsync(); // Читаем ответ сервера
                Console.WriteLine($"Ответ сервера: {responseMessage}");

                // Отправляем сообщение на другой сервер (если это необходимо)
                await SendMessageToExternalServer(message);
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.StatusCode}");
            }


            // GET - запрос для получения имени сервера
            var nameResponse = await client.GetAsync($"{baseUrl}/name");
            if (nameResponse.IsSuccessStatusCode)
            {
                var serverName = await nameResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Имя сервера: {serverName}");
            }
            else
            {
                Console.WriteLine($"Ошибка: {nameResponse.StatusCode}");
            }
        }
    }

    // Отправка сообщения на другой сервер
    public static async Task SendMessageToExternalServer(string message)
    {
        
        using (var client = new HttpClient())
        {
            var baseUrl = "http://172.29.9.90:3400/message"; // Путь / адрес
            Console.WriteLine($"Отправка сообщения на {baseUrl}: {message}");

            var messageModel = new { message = message }; // messageModel объект для отправки
            var json = JsonSerializer.Serialize(messageModel);

            var content = new StringContent(json, Encoding.UTF8, "application/json"); // JSON в StringContent
            int massageAge = 4;
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i <= massageAge; i++)
            {
                
                // POST запрос
                var response = await client.PostAsync(baseUrl, content);

                if (response.IsSuccessStatusCode) // Проверка запроса
                {
                    //Console.WriteLine($"Сообщение доставлено: {baseUrl}");
                    Console.WriteLine($"Сообщение номер {i}");
                }
                else
                {
                    Console.WriteLine($"Ошибка доставки сообщения: {response.StatusCode}");
                }
            }
            stopwatch.Stop(); // Остановка таймера
            await Task.Delay(1); //задержка в 1 миллсек.
            Console.WriteLine($"Отправлено {massageAge} сообщений за {stopwatch.Elapsed.TotalSeconds} сек.");
        }
    }
}

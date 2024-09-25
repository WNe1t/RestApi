using System.Text;
using System.Text.Json;

class MessageClientProgram
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://172.29.13.124:5273"; // URL сервера
        using var client = new HttpClient(); // HttpClient для отправки запросов
        SendingMessages sendingMessages = new SendingMessages();
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
                sendingMessages.SendMessageToExternalServer(message);
                await sendingMessages.ResendingMessages(async () => await sendingMessages.SendMessageToExternalServer(message));
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
                var responseMessage = await response.Content.ReadAsStringAsync(); // Читаем ответ сервера
                Console.WriteLine($"Ответ сервера: {responseMessage}");

                // Отправляем сообщение на другой сервер (если это необходимо)
                sendingMessages.SendMessageToExternalServer(message);
            }
            else
            {
                Console.WriteLine($"Ошибка: {nameResponse.StatusCode}");
            }
        }
    }

    
}

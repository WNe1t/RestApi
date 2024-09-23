using System.Text;
using System.Text.Json;

class MessageClientProgram
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://172.29.13.124:5273";
        using var client = new HttpClient(); //HttpClient() - класс для запросов http - GET, POST...

        while (true)
        {
            Console.Write("Сообщение:'exit' - выход: ");
            var message = Console.ReadLine();

            if (message?.ToLower() == "exit")
            {
                break;
            }

            // Создаем объект для отправки
            var messageModel = new { message = message }; // Изменяем поле на "message", чтобы оно соответствовало серверу
            var json = JsonSerializer.Serialize(messageModel);

            // POST - запрос
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // StringContent - отправляет, упаковывает JSON
            var response = await client.PostAsync($"{baseUrl}/message", content); // PostAsync метод класса HttpClient, для выполнения post запроса
            
            if (response.IsSuccessStatusCode) // Проверяем, был ли запрос успешным
            {
                var responseMessage = await response.Content.ReadAsStringAsync(); // Читаем содержимое ответа сервера
                Console.WriteLine($"Ответ сервера: {responseMessage}");
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.StatusCode}");
            }

            // GET - запрос
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
}

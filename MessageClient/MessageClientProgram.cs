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
            var messageModel = new { Text = message };
            var json = JsonSerializer.Serialize(messageModel);

            //POST - запрос
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // StringContent - отправляет,упаковывает JSON
            var response = await client.PostAsync($"{baseUrl}/message", content); //PostAsync метод класса HttpClient, для выполнения post запроса
            //baseUrl/message - базовый URL сервера/путь , content - содержит обьект StringContent строковое сообщение из Cmd
            if (response.IsSuccessStatusCode) //свойство обьекта указывающее был ли запрос успешным
            {
                var responseMessage = await response.Content.ReadAsStringAsync(); // обьект_запрос.свойство_обьекта.метод читает содержимое Content
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

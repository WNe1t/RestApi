using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class MessageClientProgram
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://localhost:5273";
        using var client = new HttpClient();

        while (true)
        {
            Console.Write("Сообщение ('exit' - выход): ");
            var message = Console.ReadLine();

            if (message?.ToLower() == "exit")
            {
                break;
            }

            var content = new StringContent(message, Encoding.UTF8, "text/plain");
            var response = await client.PostAsync($"{baseUrl}/message", content);

            if (response.IsSuccessStatusCode)
            {
                var responseMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ сервера: {responseMessage}");
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.StatusCode}");
            }

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

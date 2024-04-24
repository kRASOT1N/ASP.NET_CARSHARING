using System.Web;

namespace MyAspNetApp
{
    public class Program
    {
        private static readonly CarSharingDatabase carSharingDatabase = new("your_database_name.db");
        public static CarSharingDatabase _carDatabase = carSharingDatabase; // Сделали статическим

        static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
    

            app.MapGet("/openBron", () =>
                {
                    var html = File.ReadAllText("bron.html");
                    return Results.Content(html, "text/html");
                });


            app.MapGet("/", () =>
            {
                var html = File.ReadAllText("main.html");
                return Results.Content(html, "text/html");
            });

            app.MapGet("/openCarlist", () =>
            {
                var html = File.ReadAllText("carlist.html");
                return Results.Content(html, "text/html");
            });

            app.MapPost("/submitBooking", async (HttpRequest request) =>
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    // Предполагаем, что данные приходят в формате "name=Имя&phone=Телефон&email=Email"
                    var data = HttpUtility.ParseQueryString(body);
                    var name = data["name"];
                    var phone = data["phone"];
                    var email = data["email"];
                    var message = $"New Booking:\nName: {name}\nPhone: {phone}\nEmail: {email}";

                    await SendMessageToTelegram(message);
                }
                var html = File.ReadAllText("main.html");
                return Results.Content(html, "text/html");
            });


            app.MapGet("/openBase", () =>
            {
                // Получаем данные из базы данных
                var carsData = _carDatabase.GetCarsData();

                // Генерируем HTML-таблицу
                var html = "<!DOCTYPE html><html lang=\"ru\"><style> .nav {\r\n            width: 100%;\r\n            background-color: #333;\r\n            color: white;\r\n            text-align: center;\r\n            padding: 10px 0;\r\n        }\r\n        .nav a {\r\n            color: white;\r\n            padding: 10px 20px;\r\n            text-decoration: none;\r\n            font-size: 1.2em;\r\n        }</style><head><meta charset=\"UTF-8\"><title>Car Database</title></head><body><body>\r\n    <div class=\"nav\">\r\n        <a href=\"/\">Главная</a>\r\n        <a href=\"/openCarlist\">Наши машины</a>\r\n    </div> <h1>Состояние машин</h1><table border=\"1\"><tr><th>ID</th><th>Модель</th><th>Год</th><th>Категория</th><th>Статус</th></tr>";

                foreach (var car in carsData)
                {
                    html += "<tr>";
                    html += $"<td>{car["ID"]}</td>";
                    html += $"<td>{car["Model"]}</td>";
                    html += $"<td>{car["Year"]}</td>";
                    html += $"<td>{car["Category"]}</td>";
                    html += $"<td>{car["Status"]}</td>";
                    html += "</tr>";
                }

                html += "</table></body></html>";

                // Возвращаем HTML-таблицу в качестве ответа
                return Results.Content(html, "text/html");
            });

            app.Run();
        }

        private static async Task SendMessageToTelegram(string message)
        {
            var httpClient = new HttpClient();
            var botToken = "7160610788:AAHMFeZED_6hwFvlWAhl4rwfE4pn4DFxlHM";
            var chatId = "-1002083582134";
            var url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error sending message to Telegram");
            }
        }

    }
}
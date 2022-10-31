using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NSwagGrocerySdk;
namespace MyConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Press any key when you're ready!");
            Console.ReadKey();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "<YOUR_TOKEN>");

            var groceriesClient = new Client("https://localhost:7013/", client);

            var items = await groceriesClient.GetGroceryItemsAsync();

            foreach (var item in items)
            {
                Console.WriteLine($"* Id:{item.Id} Name:{item.Name} Description:{item.Description} Quantity:{item.Quantity}");
            }

            Console.WriteLine("Adding a new item");
            await groceriesClient.AddGroceryItemAsync(new GroceryItem("Red or White",0,"Wine", 2));

            items = await groceriesClient.GetGroceryItemsAsync();

            Console.WriteLine("Updated list");
            foreach (var item in items)
            {
                Console.WriteLine($"* Id:{item.Id} Name:{item.Name} Description:{item.Description} Quantity:{item.Quantity}");
            }
            
            Console.WriteLine("Press any key when you're ready!");
            Console.ReadKey();
        }
    }
}

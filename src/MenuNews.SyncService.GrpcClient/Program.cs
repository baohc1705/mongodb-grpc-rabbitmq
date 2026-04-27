using Grpc.Net.Client;
using MenuNews.SyncService.GrpcClient.Protos;

namespace MenuNews.SyncService.GrpcClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting gRPC Client...");


        using var channel = GrpcChannel.ForAddress("http://localhost:5002");

        var client = new NewsService.NewsServiceClient(channel);

        try
        {
            Console.WriteLine("Calling GetAllNews...");
            
            // Call the GetAllNews
            var response = await client.GetAllNewsAsync(new GetAllNewsRquest());

            Console.WriteLine($"Received {response.Items.Count} news items:");
            Console.WriteLine("--------------------------------------------------");

            foreach (var item in response.Items)
            {
                Console.WriteLine($"ID: {item.Id}");
                Console.WriteLine($"Title: {item.Title}");
                Console.WriteLine($"Slug: {item.Slug}");
                Console.WriteLine($"Status: {item.Status}");
                Console.WriteLine($"Summary: {item.Summary}");
                Console.WriteLine($"Published At: {item.PublishedAt}");
                Console.WriteLine($"Is Active: {item.IsActive}");
                
                if (item.Menus.Count > 0)
                {
                    Console.WriteLine("Menus:");
                    foreach (var menu in item.Menus)
                    {
                        Console.WriteLine($"  - {menu.Name} (Order: {menu.DisplayOrder}, NM Order: {menu.NmDisplayOrder})");
                    }
                }
                else
                {
                    Console.WriteLine("Menus: None");
                }
                
                Console.WriteLine("--------------------------------------------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

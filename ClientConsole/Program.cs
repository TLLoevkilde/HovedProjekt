var client = new HttpClient();

Console.WriteLine("Requesting message from ResourceApi");
var response = await client.GetAsync("https://localhost:7126/api/message");
var content = await response.Content.ReadAsStringAsync();

Console.WriteLine(content);
Console.WriteLine("\nPress enter to exit...");
Console.ReadLine();
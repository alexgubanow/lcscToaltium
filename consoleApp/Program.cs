using System;
using System.Net.Http;
using System.Text.Json;

namespace consoleApp
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var responseString = await client.GetStringAsync("https://easyeda.com/api/components/8dc274d742c945d894162854d2ea79f5");
            var response = JsonSerializer.Deserialize<lcsclib.GenericResponse>(responseString);
            if (response.success == true)
            {
                var component = JsonSerializer.Deserialize<lcsclib.FootprintResponse>(response.result.GetRawText());
            }
            Console.WriteLine("Hello World!");
        }
    }
}
/*public static T ToObject<T>(this JsonElement element)
{
    var json = element.GetRawText();
    return JsonSerializer.Deserialize<T>(json);
}
public static T ToObject<T>(this JsonDocument document)
{
    var json = document.RootElement.GetRawText();
    return JsonSerializer.Deserialize<T>(json);
}*/
using lcsclib;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace consoleApp
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            await RequestAndSave("https://easyeda.com/api/components/8dc274d742c945d894162854d2ea79f5");
            await RequestAndSave("https://easyeda.com/api/components/0be27f9ebfef4a48b228771721089682");
            await RequestAndSave("https://easyeda.com/api/components/3ae51f9fa3264b7ab826ce43677ca53e");
            await RequestAndSave("https://easyeda.com/api/components/437735bee9444a5d9dd435521a21a6af");
            Console.WriteLine("press any key to disappear");
            Console.ReadKey();
        }

        private static async Task RequestAndSave(string requestStr)
        {
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync(requestStr);
            var response = JsonSerializer.Deserialize<lcsclib.GenericResponse>(responseString);
            if (response.success == true)
            {
                var component = JsonSerializer.Deserialize<lcsclib.FootprintResponse>(response.result.GetRawText());
                var pcbComponent = Converters.FootprintResponseToPcbComponent(component);
                Converters.SavePcbComponentToFile(pcbComponent);
            }
            Console.WriteLine("request is: {0}\nresponse code: {1}", requestStr, response.code);
        }
    }
}
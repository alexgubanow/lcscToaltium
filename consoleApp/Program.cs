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
            await RequestAndSave("https://easyeda.com/api/components/437735bee9444a5d9dd435521a21a6af");//micro-usb
            await RequestAndSave("https://easyeda.com/api/components/da48df861c744d6795fcb3db9fd9f7b4");//CAP-SMD_BD10.0-L10
            await RequestAndSave("https://easyeda.com/api/components/3be10a76f83f422aa182e110e9d3dfed");//sop-5
            await RequestAndSave("https://easyeda.com/api/components/095c98d119cf4f3089d4e82d07b27c72");//WJ301V-5.0-2P
            await RequestAndSave("https://easyeda.com/api/components/2ada8f4ae55543e0922c8ee62e571dd5");//led0603
            await RequestAndSave("https://easyeda.com/api/components/bbc61cb2bd3840b290cf4f8db68b3c00");//c0805

            Console.WriteLine("press any key to disappear");
            Console.ReadKey();
        }

        private static async Task RequestAndSave(string requestStr)
        {
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync(requestStr);
            var response = JsonSerializer.Deserialize<lcsclib.GenericResponse>(responseString);
            Console.WriteLine("request is: {0}\nresponse code: {1}", requestStr, response.code);
            if (response.success == true)
            {
                var component = JsonSerializer.Deserialize<lcsclib.FootprintResponse>(response.result.GetRawText());
                try
                {
                    var pcbComponent = Converters.FootprintResponseToPcbComponent(component);
                    Converters.SavePcbComponentToFile(pcbComponent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to process: {0}", ex.Message);
                }
            }
        }
    }
}
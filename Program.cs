using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sumit.Model;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

internal class Weatherapp
{
    static async Task ProcessRepositoriesAsync(string latitude, string longitude)
    {
        using (var client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage =
                await client.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true");
            string jsonResult = httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var oResult = JsonConvert.DeserializeObject<GeoLocationData>(jsonResult);
            Console.WriteLine($"Temparature={oResult.Current_Weather.Temperature}, Wind speed={oResult.Current_Weather.WindSpeed}\n" +
                $"Latitude={oResult.Latitude}, Longitude={oResult.Longitude}");
        }

    }
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the city name");
        string cityName = Console.ReadLine();

        string cityDetails = File.ReadAllText(@"./Data/CityList.json");
        JObject oCityDetails = JObject.Parse(cityDetails);

        var selectedCityData = from item in oCityDetails["cities"]
                               where ((string)item["city"]).ToUpper() == cityName.ToUpper()
                               select item;
        if (!selectedCityData.Any())
        {
            Console.WriteLine("Invalid City Name");
        }
        else
        {
            var filterLatLng = from data in selectedCityData select new { latitude = data["lat"], longitude=data["lng"] };
            var selectedLatitude =filterLatLng.First().latitude;
            var selectedLongitude = filterLatLng.First().longitude;

            ProcessRepositoriesAsync(selectedLatitude.ToString(),selectedLongitude.ToString()).Wait();
        }
        Console.ReadLine();

    }
}
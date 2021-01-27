using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Weather
{
    public struct WeatherData
    {
        public int id;
        public string main;
        public string description;
        public string icon;
        public WeatherData(int _id, string _main, string _description, string _icon)
        {
            id = _id;
            main = _main;
            description = _description;
            icon = _icon;
        }

    }
    public struct WeatherDataRoot
    {
        [NonSerialized] public bool initialized;
        public WeatherData[] weather;
        public WeatherDataRoot(WeatherData[] _weather)
        {
            weather = _weather;
            initialized = true;
        }
    }
    public static class WeatherFinder
    {
        public static async Task<WeatherDataRoot> GetWeatherData(string CityName)
        {
            List<string> nonoKeys = new List<string>();
            nonoKeys.Add("None");
            nonoKeys.Add("");
            nonoKeys.Add("Empty");
            nonoKeys.Add("ApiKey");
            nonoKeys.Add(" ");
            bool validKey = nonoKeys.Any(s => PluginConfig.Instance.WeatherFinder.apiKey.Contains(s));
            
            if (!validKey || !PluginConfig.Instance.WeatherFinder.enabled)
            {
                WeatherData weatherData = new WeatherData(0, "", "", "");
                List<WeatherData> datas = new List<WeatherData>();
                datas.Add(weatherData);
                return new WeatherDataRoot(datas.ToArray());
            }
            //Make sure there are no issues
            CityName = CityName.Replace(" ", "%20");
            CityName = CityName.Replace(",", "%2C");

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://community-open-weather-map.p.rapidapi.com/weather?q=" + CityName),
                Headers =
                {
                    { "x-rapidapi-key", PluginConfig.Instance.WeatherFinder.apiKey },
                    { "x-rapidapi-host", "community-open-weather-map.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                return JsonConvert.DeserializeObject<WeatherDataRoot>(body);
            }
        }

        public static async Task<WeatherDataRoot> GetWeatherData()
        {
            List<string> nonoKeys = new List<string>();
            nonoKeys.Add("None");
            nonoKeys.Add("");
            nonoKeys.Add("Empty");
            nonoKeys.Add("ApiKey");
            nonoKeys.Add(" ");
            bool validKey = nonoKeys.Any(s => PluginConfig.Instance.WeatherFinder.apiKey.Contains(s));

            if (!validKey || !PluginConfig.Instance.WeatherFinder.enabled)
            {
                WeatherData weatherData = new WeatherData(0, "", "", "");
                List<WeatherData> datas = new List<WeatherData>();
                datas.Add(weatherData);
                return new WeatherDataRoot(datas.ToArray());
            }
            WeatherDataRoot data =  await GetWeatherData(PluginConfig.Instance.WeatherFinder.cityName);
            return data;
        }
    }
}

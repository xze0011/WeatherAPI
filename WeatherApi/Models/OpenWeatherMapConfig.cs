namespace WeatherApi.Models
{
    public class OpenWeatherMapConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public List<string> ApiKeys { get; set; } = new List<string>();
    }
}

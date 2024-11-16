namespace WeatherApi.Models
{
    public class RateLimitConfig
    {
        public List<string> ClientKeys { get; set; } = new List<string>();
        public int TokenCapacity { get; set; } = 5; 
    }
}

namespace api.Models
{
    public class OpenWeatherMapApiKeys
    {
        public List<string> ApiKeys { get; }

        public OpenWeatherMapApiKeys(List<string> apiKeys)
        {
            ApiKeys = apiKeys;
        }
    }
}

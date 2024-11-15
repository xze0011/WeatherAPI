public class Token
{
    public string Key { get; set; }
    public int RemainingUses { get; set; }
    public DateTime LastRefillTime { get; set; }
    private readonly int _capacity;

    public Token(string key, int capacity)
    {
        Key = key;
        RemainingUses = capacity;
        _capacity = capacity;
        LastRefillTime = DateTime.UtcNow;
    }

    public void Refill(TimeSpan refillInterval)
    {
        if (DateTime.UtcNow - LastRefillTime >= refillInterval)
        {
            RemainingUses = _capacity;
            LastRefillTime = DateTime.UtcNow;
        }
    }
}

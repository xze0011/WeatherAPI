namespace api.Interfaces
{
    public interface IRateLimitService
    {
        bool TryConsumeToken();
    }
}

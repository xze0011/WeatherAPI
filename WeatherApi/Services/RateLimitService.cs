using api.Interfaces;
using api.Models;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly TokenBucket _tokenBucket;
        private readonly TimeSpan _refillInterval = TimeSpan.FromHours(1);
        private readonly object _lock = new object();
        private readonly ILogger<RateLimitService> _logger;

        public RateLimitService(TokenBucket tokenBucket, ILogger<RateLimitService> logger)
        {
            _tokenBucket = tokenBucket ?? throw new ArgumentNullException(nameof(tokenBucket));
            _logger = logger;
        }

        /// <summary>
        /// Attempts to consume a token from the token bucket.
        /// </summary>
        /// <returns>True if a token was consumed; otherwise, false.</returns>
        public bool TryConsumeToken()
        {
            lock (_lock)
            {
                foreach (var token in _tokenBucket.Tokens)
                {
                    token.Refill(_refillInterval);

                    if (token.RemainingUses > 0)
                    {
                        token.RemainingUses--;
                        _logger.LogInformation("Token consumed. Remaining uses: {RemainingUses}", token.RemainingUses);
                        return true;
                    }
                }

                _logger.LogWarning("Rate limit exceeded. No tokens available.");
                return false;
            }
        }
    }
}

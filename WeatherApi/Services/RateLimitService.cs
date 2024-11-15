using api.Interfaces;
using api.Models;
using System;
using System.Threading;

namespace api.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly TokenBucket _tokenBucket;
        private readonly TimeSpan _refillInterval = TimeSpan.FromHours(1);
        private readonly object _lock = new object();

        public RateLimitService(TokenBucket tokenBucket)
        {
            Console.WriteLine($"[RateLimitService] Initialized with provided TokenBucket.");
            _tokenBucket = tokenBucket;
        }

        public bool TryConsumeToken()
        {
            lock (_lock)
            {
                foreach (var token in _tokenBucket.Tokens)
                {
                    token.Refill(_refillInterval);

                    if (token.RemainingUses > 0)
                    {
                        // Consume one token
                        token.RemainingUses--;
                        Console.WriteLine($"[RateLimitService] Token consumed - Key: {token.Key}, RemainingUses: {token.RemainingUses}");
                        return true;
                    }
                }
                Console.WriteLine("[RateLimitService] All tokens are exhausted.");
            }

            // All tokens are exhausted
            return false;
        }
    }
}

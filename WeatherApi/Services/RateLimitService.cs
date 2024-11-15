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
            Console.WriteLine($"[RateLimitService] Constructor called with TokenBucket: {tokenBucket}");
            _tokenBucket = tokenBucket;
        }

        public bool TryConsumeToken()
        {
            Console.WriteLine("[RateLimitService] TryConsumeToken() method called.");
            lock (_lock)
            {
                Console.WriteLine("[RateLimitService] Entering lock region.");
                foreach (var token in _tokenBucket.Tokens)
                {
                    Console.WriteLine($"[RateLimitService] Checking token - Key: {token.Key}, RemainingUses: {token.RemainingUses}");

                    token.Refill(_refillInterval);
                    Console.WriteLine($"[RateLimitService] After Refill() call, token status - Key: {token.Key}, RemainingUses: {token.RemainingUses}");

                    if (token.RemainingUses > 0)
                    {
                        // Consume one token
                        token.RemainingUses--;
                        Console.WriteLine($"[RateLimitService] Successfully consumed one token - Key: {token.Key}, RemainingUses: {token.RemainingUses}");
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

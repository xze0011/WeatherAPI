using System;
using System.Collections.Generic;
using api.Interfaces;
using api.Models;
using api.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace api.Tests.Services
{
    public class RateLimitServiceTests
    {
        private readonly TokenBucket _tokenBucket;
        private readonly ILogger<RateLimitService> _fakeLogger;
        private readonly RateLimitService _rateLimitService;

        public RateLimitServiceTests()
        {
            _fakeLogger = A.Fake<ILogger<RateLimitService>>();
            _tokenBucket = new TokenBucket(new List<string> { "TestKey1", "TestKey2", "TestKey3", "TestKey4", "TestKey5" }, 5);
            _rateLimitService = new RateLimitService(_tokenBucket, _fakeLogger);
        }

        [Fact]
        public void TryConsumeToken_ShouldReturnTrue_WhenTokensAreAvailable()
        {
            // Act
            var results = new List<bool>();
            for (int i = 0; i < 5; i++)
            {
                results.Add(_rateLimitService.TryConsumeToken());
            }

            // Assert
            results.Should().AllBeEquivalentTo(true); // All attempts should succeed
            _tokenBucket.Tokens[0].RemainingUses.Should().Be(0); // Verify that the first token's remaining uses are reduced to 0
        }

        [Fact]
        public void TryConsumeToken_ShouldReturnFalse_WhenNoTokensAreAvailable()
        {
            // Consume all tokens
            for (int i = 0; i < 26; i++)
            {
                _rateLimitService.TryConsumeToken();
            }

            // Act
            var result = _rateLimitService.TryConsumeToken();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TryConsumeToken_ShouldRefillTokens_AfterRefillInterval()
        {
            // Consume all tokens
            for (int i = 0; i < 5; i++)
            {
                _rateLimitService.TryConsumeToken();
            }

            // Simulate passing the refill interval by setting the LastRefill time to 1 hour ago
            foreach (var token in _tokenBucket.Tokens)
            {
                token.LastRefillTime = DateTime.UtcNow - TimeSpan.FromHours(1);
            }

            // Act
            var result = _rateLimitService.TryConsumeToken();

            // Assert
            result.Should().BeTrue();
            _tokenBucket.Tokens[0].RemainingUses.Should().Be(4); // Verify that the first token was refilled with 5 uses, 1 used
        }
    }
}

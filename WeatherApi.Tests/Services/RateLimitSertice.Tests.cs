using api.Models;
using api.Services;

namespace api.Tests.Services
{
    public class RateLimitServiceTests
    {
        private readonly RateLimitService _rateLimitService;
        private readonly TokenBucket _tokenBucket;

        public RateLimitServiceTests()
        {
            var keys = new List<string> { "TestKey1", "TestKey2" };
            int capacity = 1;

            _tokenBucket = new TokenBucket(keys, capacity);

            // 创建 RateLimitService 实例
            _rateLimitService = new RateLimitService(_tokenBucket);
        }

        [Fact]
        public void TryConsumeToken_ShouldReturnTrue_WhenTokenIsAvailable()
        {
            // No Arrage 

            // Act
            var result = _rateLimitService.TryConsumeToken();

            // Assert
            Assert.True(result);
            Assert.Equal(0, _tokenBucket.Tokens[0].RemainingUses); // 确保令牌被消耗
        }

        [Fact]
        public void TryConsumeToken_ShouldReturnFalse_WhenAllTokensAreExhausted()
        {
            // Arrange
            _tokenBucket.Tokens[0].RemainingUses = 0; // 将所有令牌的使用次数设为0
            _tokenBucket.Tokens[1].RemainingUses = 0;

            // Act
            var result = _rateLimitService.TryConsumeToken();

            // Assert
            Assert.False(result);
        }

    }
}

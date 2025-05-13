namespace RateLimitSharp.Tests;

public class Tests {
    [Fact]
    public void ManualKeyedRateLimiterTest() {
        using ManualKeyedRateLimiter rateLimiter = new(limit: 3);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(false);
        rateLimiter.Decrease("key");
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(false);
    }
    [Fact]
    public void TokenBucketKeyedRateLimiterTest() {
        using TokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, interval: TimeSpan.FromSeconds(1.0));
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(false);
        Thread.Sleep(TimeSpan.FromSeconds(2.0));
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(false);
    }
    [Fact]
    public void IncrementalTokenBucketKeyedRateLimiterTest() {
        using IncrementalTokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, interval: TimeSpan.FromSeconds(1.0));
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(true);
        rateLimiter.TryIncrease("key").ShouldBe(false);
        Thread.Sleep(TimeSpan.FromSeconds(0.5));
        rateLimiter.TryIncrease("key").ShouldBe(true);
    }
}
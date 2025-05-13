namespace RateLimitSharp.Tests;

public class Tests {
    [Fact]
    public void ManualKeyedRateLimiterTest() {
        using ManualKeyedRateLimiter rateLimiter = new(limit: 3);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(false);
        rateLimiter.Release("key");
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(false);
    }
    [Fact]
    public void TokenBucketKeyedRateLimiterTest() {
        using TokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, interval: TimeSpan.FromSeconds(1.0));
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(false);
        Thread.Sleep(TimeSpan.FromSeconds(2.0));
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(false);
    }
    [Fact]
    public void IncrementalTokenBucketKeyedRateLimiterTest() {
        using IncrementalTokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, incrementalInterval: TimeSpan.FromSeconds(1.0));
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(false);
        Thread.Sleep(TimeSpan.FromSeconds(1.5));
        rateLimiter.TryAcquire("key").ShouldBe(true);
        rateLimiter.TryAcquire("key").ShouldBe(false);
    }
}
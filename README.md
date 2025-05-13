# RateLimitSharp

[![NuGet](https://img.shields.io/nuget/v/RateLimitSharp.svg)](https://www.nuget.org/packages/RateLimitSharp)

Simple rate limiters for resources in C#.

This library was created in response to the over-engineered rate limiters in [`System.Threading.RateLimiting`](https://www.nuget.org/packages/System.Threading.RateLimiting).

## Types

The recommended rate limiter for most cases is `IncrementalTokenBucketKeyedRateLimiter`.

### ManualKeyedRateLimiter

A rate limiter per key that you release manually.

```cs
using ManualKeyedRateLimiter rateLimiter = new(limit: 3);

// Add a claim for John Doe.
bool available = rateLimiter.TryAcquire("John Doe");

// Remove a claim from John Doe.
rateLimiter.Release("John Doe");
```

### TokenBucketKeyedRateLimiter

A rate limiter per key that automatically releases claims a fixed interval after being acquired.

```cs
using TokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, interval: TimeSpan.FromSeconds(1.0));

// Add two claims for John Doe.
bool available = rateLimiter.TryAcquire("John Doe", amount: 2);

// Both claims will be removed after 1 second.
```

### IncrementalTokenBucketKeyedRateLimiter

A rate limiter per key that automatically releases a claim in a queued manner every interval.

```cs
using IncrementalTokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, incrementalInterval: TimeSpan.FromSeconds(1.0));

// Add two claims for John Doe.
bool available = rateLimiter.TryAcquire("John Doe", amount: 2);

// One claim will be removed after 1 second and another will be removed after 2 seconds.
```
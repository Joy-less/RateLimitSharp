# RateLimitSharp

[![NuGet](https://img.shields.io/nuget/v/RateLimitSharp.svg)](https://www.nuget.org/packages/RateLimitSharp)

Simple rate limiters for resources in C#.

This library was created in response to the over-engineered rate limiters in [`System.Threading.RateLimiting`](https://www.nuget.org/packages/System.Threading.RateLimiting).

## Types

The recommended rate limiter for most cases is `IncrementalTokenBucketKeyedRateLimiter`.

### ManualKeyedRateLimiter

A rate limiter per key that you replace manually.

```cs
using ManualKeyedRateLimiter rateLimiter = new(limit: 3);

// Add a claim for John Doe.
bool available = rateLimiter.TryAcquire("John Doe");

// Remove a claim from John Doe.
rateLimiter.Release("John Doe");
```

### TokenBucketKeyedRateLimiter

A rate limiter per key that automatically decreases claims a fixed interval after being increased.

```cs
using TokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, interval: TimeSpan.FromSeconds(1.0));

// Add two claims for John Doe.
bool available = rateLimiter.TryAcquire("John Doe", amount: 2);

// All claims will be removed after 1 second.
```

### IncrementalTokenBucketKeyedRateLimiter

A rate limiter per key that automatically decreases claims gradually over the period of the interval.

```cs
using IncrementalTokenBucketKeyedRateLimiter rateLimiter = new(limit: 3, interval: TimeSpan.FromSeconds(1.0));

// Add two claims for John Doe.
bool available = rateLimiter.TryAcquire("John Doe", amount: 2);

// All claims will be removed gradually over the course of 1 second.
```
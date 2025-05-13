// Compatibility
#if !NET9_0_OR_GREATER
using Lock = object;
#endif

namespace RateLimitSharp;

/// <summary>
/// A keyed rate limiter that increments a counter and decrements it after a specified interval.<br/>
/// Unlike <see cref="IncrementalTokenBucketKeyedRateLimiter"/>, tokens are released a fixed interval after being acquired, meaning tokens may be released in bursts.
/// </summary>
public class TokenBucketKeyedRateLimiter : IKeyedRateLimiter {
    /// <summary>
    /// The maximum number of claims.
    /// </summary>
    public long Limit { get; }
    /// <summary>
    /// The interval before a claim is automatically released.
    /// </summary>
    public TimeSpan Interval { get; }

    /// <summary>
    /// The lock used to access resources in this rate limiter.
    /// </summary>
    private readonly Lock Lock = new();
    /// <summary>
    /// The claim counters for each key.
    /// </summary>
    private readonly Dictionary<object, long> Counters = [];
    /// <summary>
    /// The canceller for the release tasks.
    /// </summary>
    private CancellationTokenSource CancelTokenSource = new();

    /// <summary>
    /// Constructs a keyed token bucket.
    /// </summary>
    /// <param name="limit">
    /// The maximum number of claims.
    /// </param>
    /// <param name="interval">
    /// The interval before a claim is automatically released.
    /// </param>
    public TokenBucketKeyedRateLimiter(long limit, TimeSpan interval) {
        Limit = limit;
        Interval = interval;
    }
    /// <summary>
    /// Returns the remaining number of claims for the key.
    /// </summary>
    public long GetRemaining(object key) {
        lock (Lock) {
            return Limit - Counters.GetValueOrDefault(key);
        }
    }
    /// <summary>
    /// Adds the specified number of claims for the key if possible.
    /// </summary>
    public bool TryAcquire(object key, long amount = 1) {
        // Ensure amount >= 0
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, nameof(amount));

        lock (Lock) {
            // Get counter
            long Counter = Counters.GetValueOrDefault(key);

            // Ensure new counter is under rate limit
            if (Counter + amount > Limit) {
                return false;
            }

            // Increase counter
            Counter += amount;
            Counters[key] = Counter;

            // Decrease counter after interval
            _ = ScheduleReleaseAsync(key, amount);
            return true;
        }
    }
    /// <summary>
    /// Removes the specified number of claims from the key.
    /// </summary>
    public void Release(object key, long amount = 1) {
        // Ensure amount >= 0
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0, nameof(amount));

        lock (Lock) {
            // Get counter
            if (Counters.TryGetValue(key, out long Counter)) {
                // Decrease counter
                Counter -= amount;
                Counters[key] = Counter;

                // Free memory
                if (Counter <= 0) {
                    Counters.Remove(key);
                }
            }
        }
    }
    /// <summary>
    /// Resets the number of claims for every key.
    /// </summary>
    public void Reset() {
        lock (Lock) {
            // Remove all counters
            Counters.Clear();
            // Cancel release tasks
            CancelTokenSource.Cancel();
            CancelTokenSource.Dispose();
            CancelTokenSource = new CancellationTokenSource();
        }
    }
    /// <summary>
    /// Cleans up resources.
    /// </summary>
    public void Dispose() {
        GC.SuppressFinalize(this);
        // Cancel release tasks
        CancelTokenSource.Cancel();
        CancelTokenSource.Dispose();
    }

    /// <summary>
    /// Schedules a task to release the specified number of claims for the key.
    /// </summary>
    private async Task ScheduleReleaseAsync(object key, long amount) {
        // Wait for release
        await Task.Delay(Interval, cancellationToken: CancelTokenSource.Token).ConfigureAwait(false);
        // Release amount added
        Release(key, amount);
    }
}
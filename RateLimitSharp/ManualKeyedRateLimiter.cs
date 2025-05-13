// Compatibility
#if !NET9_0_OR_GREATER
using Lock = object;
#endif

namespace RateLimitSharp;

/// <summary>
/// A keyed rate limiter that increments a counter.
/// </summary>
public class ManualKeyedRateLimiter : IKeyedRateLimiter {
    /// <summary>
    /// The maximum number of claims.
    /// </summary>
    public long Limit { get; }

    /// <summary>
    /// The lock used to access resources in this rate limiter.
    /// </summary>
    private readonly Lock Lock = new();
    /// <summary>
    /// The claim counters for each key.
    /// </summary>
    private readonly Dictionary<object, long> Counters = [];

    /// <summary>
    /// Constructs a keyed token bucket.
    /// </summary>
    public ManualKeyedRateLimiter(long limit) {
        Limit = limit;
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
            long counter = Counters.GetValueOrDefault(key);

            // Ensure new counter is under rate limit
            if (counter + amount > Limit) {
                return false;
            }

            // Increase counter
            counter += amount;
            Counters[key] = counter;
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
            if (Counters.TryGetValue(key, out long counter)) {
                // Decrease counter
                counter -= amount;
                Counters[key] = counter;

                // Free memory
                if (counter <= 0) {
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
        }
    }
    /// <summary>
    /// Cleans up resources.
    /// </summary>
    public void Dispose() {
        GC.SuppressFinalize(this);
    }
}
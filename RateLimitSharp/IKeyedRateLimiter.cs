namespace RateLimitSharp;

/// <summary>
/// A keyed rate limiter.
/// </summary>
public interface IKeyedRateLimiter : IDisposable {
    /// <summary>
    /// The maximum number of claims.
    /// </summary>
    public long Limit { get; }

    /// <summary>
    /// Returns the remaining number of claims for the key.
    /// </summary>
    public long GetRemaining(object key);
    /// <summary>
    /// Adds the specified number of claims for the key if possible.
    /// </summary>
    public bool TryIncrease(object key, long amount = 1);
    /// <summary>
    /// Removes the specified number of claims from the key.
    /// </summary>
    public void Decrease(object key, long amount = 1);
    /// <summary>
    /// Resets the number of claims for every key.
    /// </summary>
    public void Reset();
}
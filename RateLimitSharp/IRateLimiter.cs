namespace RateLimitSharp;

/// <summary>
/// A rate limiter.
/// </summary>
public interface IRateLimiter : IDisposable {
    /// <summary>
    /// The maximum number of claims.
    /// </summary>
    public long Limit { get; }

    /// <summary>
    /// Returns the remaining number of claims.
    /// </summary>
    public long GetRemaining();
    /// <summary>
    /// Adds the specified number of claims if possible.
    /// </summary>
    public bool TryIncrease(long amount = 1);
    /// <summary>
    /// Removes the specified number of claims.
    /// </summary>
    public void Decrease(long amount = 1);
    /// <summary>
    /// Resets the number of claims.
    /// </summary>
    public void Reset();
}
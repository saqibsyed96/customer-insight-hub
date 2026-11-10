namespace CustomerInsightHub.Tests.Fakes;

/// <summary>A clock frozen at one moment, so tests that check timestamps are predictable.</summary>
public class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

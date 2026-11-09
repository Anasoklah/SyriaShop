namespace SyriaShop.IntegrationTests;

public sealed class TestClock : TimeProvider
{
    private DateTimeOffset utcNow;

    public TestClock()
    {
        utcNow = DateTimeOffset.UtcNow;
    }

    public void Advance(TimeSpan duration)
    {
        utcNow = utcNow.Add(duration);
    }

    public void Reset()
    {
        utcNow = DateTimeOffset.UtcNow;
    }

    public override DateTimeOffset GetUtcNow() => utcNow;
}

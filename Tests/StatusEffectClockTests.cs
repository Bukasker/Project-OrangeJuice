using System;

public static class StatusEffectClockTests
{
    private static void Equal(float expected, float actual, string scenario)
    {
        if (Math.Abs(expected - actual) > 0.0001f)
            throw new Exception(scenario + ": expected " + expected + ", got " + actual);
    }
    public static void Main()
    {
        var clock = new StatusEffectClock(5f);
        Equal(0f, clock.Advance(0.5f), "No premature tick");
        Equal(1f, clock.Advance(0.5f), "First tick after one second");
        Equal(4f, clock.Advance(10f), "Long frame capped to remaining duration");
        Equal(0f, clock.Advance(1f), "No ticks after expiry");
        clock = new StatusEffectClock(2.5f);
        Equal(2.5f, clock.Advance(9f), "Final partial second is applied");
        clock = new StatusEffectClock(2f);
        Equal(0f, clock.Advance(0.75f), "Fraction before refresh");
        clock.Refresh(2f);
        Equal(1f, clock.Advance(0.25f), "Refresh preserves tick progress");
        Equal(1.75f, clock.Advance(2f), "Refreshed duration counts correctly");
        foreach (int fps in new[] { 30, 60, 144 })
        {
            clock = new StatusEffectClock(6f);
            float total = 0f;
            for (int frame = 0; frame <= fps * 6; frame++) total += clock.Advance(1f / fps);
            Equal(6f, total, "Frame-rate independence at " + fps + " FPS");
        }
        var first = new StatusEffectClock(3f);
        var second = new StatusEffectClock(3f);
        first.Advance(2f);
        Equal(3f, (float)second.Remaining, "Independent clocks per character");
        Equal(0f, second.Advance(0f), "Pause");
        Equal(0f, second.Advance(-1f), "Negative delta ignored");
        Console.WriteLine("Status effect clock: all checks passed.");
    }
}

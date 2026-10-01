using System;

// Per-character clock. Definitions never store mutable runtime state.
public sealed class StatusEffectClock
{
    public double Remaining { get; private set; }
    private double pending;

    public StatusEffectClock(float duration) { Refresh(duration); }

    // Refresh keeps partial progress towards the next tick.
    public void Refresh(float duration) { Remaining = Math.Max(0, duration); }

    // Returns elapsed seconds due for application: whole seconds, plus a final fraction.
    public float Advance(float deltaTime)
    {
        if (deltaTime <= 0 || Remaining <= 0) return 0;
        double elapsed = Math.Min(deltaTime, Remaining);
        Remaining -= elapsed;
        pending += elapsed;
        double due = Math.Floor(pending + 0.000001);
        pending -= due;
        if (Remaining <= 0.000001)
        {
            due += pending;
            pending = 0;
            Remaining = 0;
        }
        return (float)due;
    }
}

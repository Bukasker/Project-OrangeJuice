using System;
public static class EffectListTrackerTests
{
    private static void Check(bool value, string name) { if (!value) throw new Exception(name); }
    public static void Main()
    {
        var tracker = new EffectListTracker<object>();
        var poison = new object();
        var heal = new object();
        tracker.Capture(new[] { poison });
        Check(tracker.GetAdded(new[] { poison }).Count == 0, "No repeat after start");
        var added = tracker.GetAdded(new[] { poison, heal });
        Check(added.Count == 1 && added[0] == heal, "Only new entry applies");
        Check(tracker.GetAdded(new[] { poison, heal }).Count == 0, "No per-frame healing");
        Check(tracker.GetAdded(new[] { heal, poison }).Count == 0, "Reordering does not apply");
        Check(tracker.GetAdded(new[] { heal }).Count == 0, "Removal does not apply");
        Check(tracker.GetAdded(new[] { heal, poison }).Count == 1, "Re-add applies again");
        Check(tracker.GetAdded(new[] { heal, poison, poison }).Count == 1, "New duplicate refreshes once");
        Check(tracker.GetAdded(null).Count == 0, "Null list");
        Check(tracker.GetAdded(new object[] { null }).Count == 0, "Empty slot");
        Check(tracker.GetAdded(new[] { poison }).Count == 1, "Assigning empty slot applies");
        added = tracker.GetAdded(new[] { heal });
        Check(added.Count == 1 && added[0] == heal, "Replacement applies");
        tracker.Capture(new[] { poison });
        Check(tracker.GetAdded(new[] { poison }).Count == 0, "Manual apply avoids duplicate");
        Console.WriteLine("Effect list changes: all checks passed.");
    }
}

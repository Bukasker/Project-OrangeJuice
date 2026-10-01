using System.Collections.Generic;

// Compares list contents, including duplicate entries, without depending on their order.
public sealed class EffectListTracker<T> where T : class
{
    private readonly List<T> previous = new List<T>();
    private readonly List<T> unmatched = new List<T>();
    private readonly List<T> added = new List<T>();

    public void Capture(T[] items)
    {
        previous.Clear();
        if (items != null) previous.AddRange(items);
    }

    public List<T> GetAdded(T[] items)
    {
        added.Clear();
        int count = items != null ? items.Length : 0;
        bool unchanged = previous.Count == count;
        for (int i = 0; unchanged && i < count; i++)
            unchanged = EqualityComparer<T>.Default.Equals(previous[i], items[i]);
        if (unchanged) return added;

        unmatched.Clear();
        unmatched.AddRange(previous);
        added.Clear();
        if (items != null)
        {
            foreach (T item in items)
            {
                if (item == null) continue;
                int index = unmatched.IndexOf(item);
                if (index >= 0) unmatched.RemoveAt(index);
                else added.Add(item);
            }
        }
        Capture(items);
        return added;
    }
}

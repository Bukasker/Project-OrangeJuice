// Standalone tests: compile with EffectApplier.cs and EffectListTracker.cs, outside Unity.
using System;
using System.Collections.Generic;
using System.Reflection;
namespace UnityEngine
{
    public class MonoBehaviour { public T GetComponent<T>() where T : class { return null; } }
    public class TooltipAttribute : Attribute { public TooltipAttribute(string value) {} }
    public class ContextMenuAttribute : Attribute { public ContextMenuAttribute(string value) {} }
    public static class Application { public static bool isPlaying = true; }
    public static class Debug { public static void LogWarning(string value, object context) {} }
}
public class StatusEffectDefinition { public bool instant; }
public class CharacterEffects
{
    public bool isActiveAndEnabled = true;
    public readonly HashSet<StatusEffectDefinition> active = new HashSet<StatusEffectDefinition>();
    public bool HasEffect(StatusEffectDefinition effect) { return active.Contains(effect); }
}
public class TestGameObject { public bool activeInHierarchy = true; }
public class CharacterStats
{
    public float currentHealth = 100;
    public readonly TestGameObject gameObject = new TestGameObject();
    public readonly CharacterEffects Effects = new CharacterEffects();
    public int applications;
    public void ApplyEffect(StatusEffectDefinition effect)
    {
        applications++;
        if (!effect.instant) Effects.active.Add(effect);
    }
}
public static class EffectApplierLifecycleTests
{
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); }
    static void Tick(EffectApplier applier) { typeof(EffectApplier).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(applier, null); }
    public static void Main()
    {
        var poison = new StatusEffectDefinition();
        var slow = new StatusEffectDefinition();
        var heal = new StatusEffectDefinition { instant = true };
        var target = new CharacterStats();
        var applier = new EffectApplier { target = target, effects = new[] { poison, slow, heal } };
        applier.ApplyToTarget();
        Check(applier.effects.Length == 2, "Instant heal removed");
        Tick(applier);
        Check(target.applications == 3, "Cleanup does not reapply effects");
        target.Effects.active.Remove(poison);
        Tick(applier);
        Check(applier.effects.Length == 1 && applier.effects[0] == slow, "Only expired entry removed");
        Check(target.applications == 3, "Remaining effect not refreshed");
        applier.effects = new[] { slow, poison };
        Tick(applier);
        Check(target.applications == 4 && target.Effects.HasEffect(poison), "Expired effect can be re-added");
        target.Effects.active.Clear();
        Tick(applier);
        Check(applier.effects.Length == 0, "Death/clear removes entries");
        applier.applyChangesDuringPlay = false;
        applier.effects = new[] { poison };
        Tick(applier);
        Check(applier.effects.Length == 1, "Unapplied entries retained");
        applier.ApplyToTarget();
        target.Effects.active.Clear();
        Tick(applier);
        Check(applier.effects.Length == 0, "Cleanup works with live changes disabled");
        var other = new CharacterStats();
        applier.effects = new[] { slow };
        applier.ApplyTo(target);
        applier.ApplyTo(other);
        target.Effects.active.Clear();
        Tick(applier);
        Check(applier.effects.Length == 1, "Waits for all actual recipients");
        other.Effects.isActiveAndEnabled = false;
        Tick(applier);
        Check(applier.effects.Length == 0, "Disabled recipient cleanup");
        Console.WriteLine("Effect Applier lifecycle: all checks passed.");
    }
}

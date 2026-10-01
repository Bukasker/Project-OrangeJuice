using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterStats))]
public class CharacterEffects : MonoBehaviour
{
    [Tooltip("Applied at Start. Entries added or replaced during Play are also applied once.")]
    [SerializeField] private StatusEffectDefinition[] startingEffects;
    private CharacterStats stats;
    private readonly EffectListTracker<StatusEffectDefinition> listTracker = new EffectListTracker<StatusEffectDefinition>();
    private readonly List<ActiveEffect> active = new List<ActiveEffect>();
    public int ActiveCount => active.Count;
    public float MovementMultiplier { get; private set; } = 1f;

    private sealed class ActiveEffect
    {
        public StatusEffectDefinition definition;
        public StatusEffectClock clock;
        public GameObject visual;
    }

    private void Awake() { stats = GetComponent<CharacterStats>(); }
    private void Start() { ApplyStartingEffects(); }

    [ContextMenu("Apply Starting Effects (Play Mode)")]
    public void ApplyStartingEffects()
    {
        if (!Application.isPlaying) return;
        listTracker.Capture(startingEffects);
        if (startingEffects == null) return;
        foreach (var effect in startingEffects) Apply(effect);
    }
    private void Update()
    {
        Advance(Time.deltaTime);
        foreach (var effect in listTracker.GetAdded(startingEffects)) Apply(effect);
    }

    public void Apply(StatusEffectDefinition effect)
    {
        if (effect == null || !isActiveAndEnabled || stats.currentHealth <= 0f) return;
        if (effect.effectType == StatusEffectType.Heal)
        {
            stats.Heal(effect.amount);
            var burst = StatusEffectVisual.Create(effect, transform);
            if (burst != null) Destroy(burst, 0.7f);
            return;
        }
        foreach (var item in active)
        {
            if (item.definition != effect) continue;
            item.clock.Refresh(Mathf.Max(0.01f, effect.duration));
            RecalculateMovement();
            return;
        }
        active.Add(new ActiveEffect
        {
            definition = effect,
            clock = new StatusEffectClock(Mathf.Max(0.01f, effect.duration)),
            visual = StatusEffectVisual.Create(effect, transform)
        });
        RecalculateMovement();
    }

    public bool HasEffect(StatusEffectDefinition effect)
    {
        foreach (var item in active)
            if (item.definition == effect) return true;
        return false;
    }

    public void Remove(StatusEffectDefinition effect)
    {
        for (int i = active.Count - 1; i >= 0; i--)
            if (active[i].definition == effect) RemoveAt(i);
        RecalculateMovement();
    }

    public void ClearEffects()
    {
        for (int i = active.Count - 1; i >= 0; i--) RemoveAt(i);
        MovementMultiplier = 1f;
    }

    public void Advance(float deltaTime)
    {
        if (!isActiveAndEnabled) return;
        if (stats.currentHealth <= 0f) { ClearEffects(); return; }
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var item = active[i];
            if (item.definition == null) { RemoveAt(i); continue; }
            float seconds = item.clock.Advance(deltaTime);
            float amount = Mathf.Max(0f, item.definition.amount) * seconds;
            switch (item.definition.effectType)
            {
                case StatusEffectType.Regeneration: stats.Heal(amount); break;
                case StatusEffectType.Bleeding:
                case StatusEffectType.Poison: stats.ApplyDamage(amount); break;
            }
            // Die() may disable/destroy the character and clear the collection.
            if (stats == null || stats.currentHealth <= 0f || !isActiveAndEnabled)
            {
                ClearEffects();
                return;
            }
            if (item.clock.Remaining <= 0) RemoveAt(i);
        }
        RecalculateMovement();
    }

    private void RemoveAt(int index)
    {
        if (active[index].visual != null) Destroy(active[index].visual);
        active.RemoveAt(index);
    }
    private void RecalculateMovement()
    {
        MovementMultiplier = 1f;
        foreach (var item in active)
            if (item.definition != null && item.definition.effectType == StatusEffectType.Slow)
                MovementMultiplier = Mathf.Min(MovementMultiplier, Mathf.Clamp01(item.definition.movementMultiplier));
    }
    private void OnDisable() { ClearEffects(); }
}

using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    public StatusEffectDefinition[] effects;
    [Tooltip("Character receiving the effects. If empty, uses CharacterStats on this object.")]
    public CharacterStats target;
    [Tooltip("Apply the configured effects once when this component starts in Play Mode.")]
    public bool applyOnStart = true;
    [Tooltip("Apply entries added or replaced in Effects during Play, once per change.")]
    public bool applyChangesDuringPlay = true;
    private readonly EffectListTracker<StatusEffectDefinition> listTracker = new EffectListTracker<StatusEffectDefinition>();

    // Track the actual recipients: ApplyTo may target more than one character.
    private readonly Dictionary<StatusEffectDefinition, List<CharacterEffects>> applied = new Dictionary<StatusEffectDefinition, List<CharacterEffects>>();
    private readonly HashSet<StatusEffectDefinition> completed = new HashSet<StatusEffectDefinition>();
    private readonly List<StatusEffectDefinition> remainingEntries = new List<StatusEffectDefinition>();

    private void Reset() { target = GetComponent<CharacterStats>(); }
    private void Start()
    {
        listTracker.Capture(effects);
        if (applyOnStart) ApplyToTarget();
    }

    private void Update()
    {
        var added = listTracker.GetAdded(effects);
        if (applyChangesDuringPlay && added.Count > 0)
        {
            CharacterStats receiver = target != null ? target : GetComponent<CharacterStats>();
            if (ValidateTarget(receiver))
                foreach (var effect in added) ApplyAndTrack(receiver, effect);
        }
        RemoveCompletedEntries();
    }
    [ContextMenu("Apply Effects To Target (Play Mode)")]
    public void ApplyToTarget()
    {
        if (!Application.isPlaying) return;
        listTracker.Capture(effects);
        CharacterStats receiver = target != null ? target : GetComponent<CharacterStats>();
        ApplyTo(receiver);
    }

    // Can be called by an attack, potion, ability or UnityEvent.
    public void ApplyTo(CharacterStats character)
    {
        if (!ValidateTarget(character)) return;
        if (effects == null || effects.Length == 0)
        {
            Debug.LogWarning("Effect Applier: the Effects list is empty.", this);
            return;
        }
        foreach (var effect in effects) ApplyAndTrack(character, effect);
        RemoveCompletedEntries();
    }

    private void ApplyAndTrack(CharacterStats character, StatusEffectDefinition effect)
    {
        if (effect == null) return;
        character.ApplyEffect(effect);
        if (!applied.TryGetValue(effect, out var recipients))
        {
            recipients = new List<CharacterEffects>();
            applied.Add(effect, recipients);
        }
        CharacterEffects receiver = character.Effects;
        if (receiver.HasEffect(effect) && !recipients.Contains(receiver)) recipients.Add(receiver);
    }

    private void RemoveCompletedEntries()
    {
        if (applied.Count == 0) return;
        completed.Clear();
        foreach (var entry in applied)
        {
            List<CharacterEffects> recipients = entry.Value;
            for (int i = recipients.Count - 1; i >= 0; i--)
            {
                var receiver = recipients[i];
                if (receiver == null || !receiver.isActiveAndEnabled || !receiver.HasEffect(entry.Key))
                    recipients.RemoveAt(i);
            }
            if (recipients.Count == 0) completed.Add(entry.Key);
        }
        if (completed.Count == 0) return;

        remainingEntries.Clear();
        if (effects != null)
        {
            foreach (var effect in effects)
                if (effect == null || !completed.Contains(effect)) remainingEntries.Add(effect);
            if (remainingEntries.Count != effects.Length)
            {
                effects = remainingEntries.ToArray();
                // Removing finished entries must not reapply the remaining effects.
                listTracker.Capture(effects);
            }
        }
        foreach (var effect in completed) applied.Remove(effect);
    }
    private bool ValidateTarget(CharacterStats character)
    {
        if (character == null)
        {
            Debug.LogWarning("Effect Applier: assign a character to Target or put this component on a character with CharacterStats.", this);
            return false;
        }
        if (character.currentHealth <= 0f)
        {
            Debug.LogWarning("Effect Applier: the target has no health left. Effects cannot be applied to a dead character.", this);
            return false;
        }
        if (!character.gameObject.activeInHierarchy || !character.Effects.isActiveAndEnabled)
        {
            Debug.LogWarning("Effect Applier: the target or its Character Effects component is disabled.", this);
            return false;
        }
        return true;
    }
}
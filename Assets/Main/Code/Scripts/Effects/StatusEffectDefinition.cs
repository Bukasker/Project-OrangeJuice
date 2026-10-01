using UnityEngine;

public enum StatusEffectType { Heal, Regeneration, Bleeding, Poison, Slow }

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Orange Juice/Status Effect")]
public class StatusEffectDefinition : ScriptableObject
{
    public StatusEffectType effectType;
    [Tooltip("HP restored once, or HP per second for regeneration/bleeding/poison.")]
    [Min(0f)] public float amount = 5f;
    [Tooltip("Seconds. Ignored for instant healing.")]
    [Min(0.01f)] public float duration = 5f;
    [Tooltip("0.5 means half movement speed. The strongest active slow wins.")]
    [Range(0f, 1f)] public float movementMultiplier = 0.5f;
    public bool showVisuals = true;
    public Color visualColor = Color.green;
    [Tooltip("Optional replacement for the built-in particles. Destroyed when the effect ends.")]
    public GameObject visualPrefab;
    public Vector3 visualOffset = new Vector3(0f, 0.5f, 0f);
}

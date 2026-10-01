using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [Header("Basic stats")]
    public int MinHealth = 0;
    public int MaxHealth = 100;
    public int Lvl = 1;
    public float currentHealth;

    [Header("Dmg calculations")]
    public Stat AttackDamage;
    public Stat ArrowDamage;
    public Stat MagicDamage;

    public Stat ArmorMelee;
    public Stat ArmorRange;
    public Stat MagicResist;


    [Header("Healh bar slider")]
    [SerializeField] public GameObject sliderGameObject;
    [SerializeField] public Slider slider;

    protected virtual void Awake()
    {
        currentHealth = MaxHealth;
        UpdateHealthSlider();
    }
    public virtual void TakeDamage(int damage)
    {
        ApplyDamage(damage);
    }

    private CharacterEffects effects;
    public CharacterEffects Effects
    {
        get
        {
            if (effects == null) effects = GetComponent<CharacterEffects>();
            if (effects == null) effects = gameObject.AddComponent<CharacterEffects>();
            return effects;
        }
    }
    public float MovementSpeedMultiplier
    {
        get
        {
            if (effects == null) effects = GetComponent<CharacterEffects>();
            return effects != null ? effects.MovementMultiplier : 1f;
        }
    }

    public void ApplyEffect(StatusEffectDefinition effect) { Effects.Apply(effect); }

    public void Heal(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f) return;
        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        UpdateHealthSlider();
    }

    // Raw health damage, also used by status effects: no armor or attacker required.
    public void ApplyDamage(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        UpdateHealthSlider();
        if (currentHealth <= 0f)
        {
            if (effects == null) effects = GetComponent<CharacterEffects>();
            if (effects != null) effects.ClearEffects();
            Die();
        }
    }

    private void UpdateHealthSlider()
    {
        if (slider == null && sliderGameObject != null)
        {
            slider = sliderGameObject.GetComponent<Slider>();
        }

        if (slider == null)
        {
            return;
        }

        slider.minValue = MinHealth;
        slider.maxValue = MaxHealth;
        slider.value = currentHealth;
    }
    public virtual void Die()
    {

    }

}


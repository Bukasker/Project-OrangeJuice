using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [Space]
    public int Gold;
    void Start()
    {
        EquipmentManager.Instance.onEquipmentChanged += OnEquipmentChanged;
    }
    private void Awake()
    {
        currentHealth = MaxHealth;
        if (slider != null)
        {
            slider.maxValue = MaxHealth;
            slider.minValue = MinHealth;
            slider.value = MaxHealth;
        }
    }
    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        base.TakeDamage(damage);
    }
    void OnEquipmentChanged(Item newItem, Item oldItem)
    {
        if (newItem != null)
        {
            Item newEquipment = newItem;
            ArmorMelee.AddMofifier(newEquipment.MeleeArmorModifier);
            ArmorRange.AddMofifier(newEquipment.RangeArmorModifier);
            MagicResist.AddMofifier(newEquipment.MagicResistModifier);

            AttackDamage.AddMofifier(newEquipment.AttackDamageModifier);
            ArrowDamage.AddMofifier(newEquipment.ArrowDamageModifier);
            MagicDamage.AddMofifier(newEquipment.MagicDamageModifier);
        }
        if(oldItem != null)
        {
            Item oldEquipment = oldItem;
            ArmorMelee.RemoveMofifier(oldEquipment.MeleeArmorModifier);
            ArmorRange.RemoveMofifier(oldEquipment.RangeArmorModifier);
            MagicResist.RemoveMofifier(oldEquipment.MagicResistModifier);

            AttackDamage.RemoveMofifier(oldEquipment.AttackDamageModifier);
            ArrowDamage.RemoveMofifier(oldEquipment.ArrowDamageModifier);
            MagicDamage.RemoveMofifier(oldEquipment.MagicDamageModifier);
        }
    }
}

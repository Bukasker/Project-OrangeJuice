using UnityEngine;

public class PlayerStats : CharacterStats
{
    [Space]
    public int Gold;
    void Start()
    {
        EquipmentManager.Instance.onEquipmentChanged += OnEquipmentChanged;
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


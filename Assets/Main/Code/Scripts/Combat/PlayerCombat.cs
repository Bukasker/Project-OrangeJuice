using UnityEngine;

public class PlayerCombat : CharacterCombat
{
    [SerializeField] private EquipmentManager equipmentManager;

    public override void Attack(CharacterStats targesStats)
    {
        if (equipmentManager.IsSwordEquiped)
        {
            targesStats.TakeDamage(myStats.AttackDamage.GetValue() - targesStats.ArmorMelee.GetValue());
        }
        else if (equipmentManager.IsBowEquiped)
        {
            targesStats.TakeDamage(myStats.ArrowDamage.GetValue() - targesStats.ArmorRange.GetValue());
        }
        else if (equipmentManager.IsSpellEquiped)
        {
            targesStats.TakeDamage(myStats.MagicDamage.GetValue() - targesStats.MagicResist.GetValue());
        }
    }
}

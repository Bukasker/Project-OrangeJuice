public class EnemyCombat : CharacterCombat
{
    public override void Attack(CharacterStats targesStats)
    {
        if (attackCooldown <= 0f)
        {
            targesStats.TakeDamage(myStats.AttackDamage.GetValue() - targesStats.ArmorMelee.GetValue());
            targesStats.TakeDamage(myStats.ArrowDamage.GetValue() - targesStats.ArmorRange.GetValue());
            targesStats.TakeDamage(myStats.MagicDamage.GetValue() - targesStats.MagicResist.GetValue());
            attackCooldown = 1f / attackSpeed;
        }
    }
}

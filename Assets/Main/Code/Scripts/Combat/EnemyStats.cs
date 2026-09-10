using UnityEngine;
using static Hero;

public class EnemyStats : CharacterStats
{
    [SerializeField] private float gizmosScale;
    [SerializeField] private PlayerStats playerStats;
    public GameObject playerGameObject;
    public EnemyStats myStats;

    public override void TakeDamage(int damage)
    {
        var LvlDiff = Lvl - playerStats.Lvl;
        if (LvlDiff > 0)
        {
            damage = damage - (2 * ((2 * LvlDiff) / 3));
            damage = Mathf.Clamp(damage, 0, int.MaxValue);
        }
        if(damage == 0)
        {
            damage = 2;
        }
        
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        base.TakeDamage(damage);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmosScale);
    }
}

using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    public float attackSpeed = 1f;
    public float attackCooldown = 0f;
    public CharacterStats myStats;
    private void Update()
    {
        attackCooldown -= Time.deltaTime;
    }
    public virtual void Attack(CharacterStats targesStats)
    {
        if (attackCooldown <= 0f)
        {
            attackCooldown = attackSpeed;
        }

    }
}

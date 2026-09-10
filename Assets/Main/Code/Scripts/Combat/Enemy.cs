using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float _gizmosScale;

    public GameObject player;
    public EnemyStats myStats;

    public void TakeDamage()
    {
        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.Attack(myStats);//deal dmg to enemy
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _gizmosScale);
    }
}

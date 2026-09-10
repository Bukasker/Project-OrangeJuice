using UnityEngine;
public class OnCollisionDealDMG : MonoBehaviour
{
    private Enemy enemy;
    private void OnTriggerEnter2D(Collider2D col)
    {
        {
            if (col.CompareTag("Enemy") == false) return;

            enemy = col.GetComponent<Enemy>();
            enemy.TakeDamage();

            Debug.Log($"{gameObject.name} collided with {col.name}");
        }
    }
}

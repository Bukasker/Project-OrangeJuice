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

    private void Start()
    {
        currentHealth = MaxHealth;
    }
    public virtual void TakeDamage(int damage)
    {

    }
    public virtual void Die()
    {

    }

}

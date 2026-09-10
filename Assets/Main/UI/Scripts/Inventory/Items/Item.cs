using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
	[Header("Item")]
	[Space]
	public string ItemName = "New Item";
	public ItemType ItemType;
	public Sprite Icon = null;
	public int Value = 0;
	public float Weight = 0f;
	public int MaxStack = 999;
	public int ItemAmount = 1;
	public bool IsToThrowAway = true;
	public GameObject ItemPrefab;

	[TextArea]
	public string Description = "Description placeholder";

	[Header("Equipment Item")]
	[Space]
	public ArmorType ArmorType;
	public WeaponType WeaponType;
	public ToolType ToolType;
	public Sprite WeaponSpriteSheet;
	public Texture2D WeaponHitSheet;

    public Texture2D ArrmorSpriteSheet;

	public int ToolLvl;

	public int AttackDamageModifier;
	public int ArrowDamageModifier;
	public int MagicDamageModifier;

    public int MeleeArmorModifier;
    public int RangeArmorModifier;
    public int MagicResistModifier;
    public virtual void Use()
    {
        Debug.Log("Used " + ItemName);
    }
}

public enum ItemType
{
	Gold,
	Weapon,
	Apperance,
	Potion,
	Food,
	Ingridiens,
	Key
}
public enum ArmorType
{
	None,
	Ring,
	Helmet,
	Armor,
	Pants,
	Glove,
	Belt,
	Greaves,
	Boots,
	Potion
}
public enum WeaponType
{
	None,
	Sword,
	Bow,
	Arrow,
	Magic
}

public enum ToolType
{
	None,
	Axe,
	Pickaxe,
	Hoe,
	Bucket,
	Scyle,
	FishingRod,
}
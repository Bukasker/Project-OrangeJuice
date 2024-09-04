using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
	[Header("Item")]
	[Space]
	public string ItemName = "New Item";
	public ItemTypes ItemType;
	public Sprite Icon = null;
	public int Value = 0;
	public float Weight = 0f;
	public int maxStack = 999;
	public new int itemAmount = 1;
	public bool isToThrowAway = true;
	public GameObject itemPrefab;

	[TextArea]
	public string Description = "Description placeholder";

	[Header("Equipment Item")]
	[Space]
	public ArmorType armorType;
	public WeaponType weaponType;
	public ToolType toolType;
	public int toolLvl;

	public int armorModifier;

	public int attackDamageModifier;
	public int arrowDamageModifier;
	public int magicDamageModifier;

	public void EquipItem(int index)
	{
		//EquipmentMenager.Instance.Equip(this, index);
	}
}

public enum ItemTypes
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
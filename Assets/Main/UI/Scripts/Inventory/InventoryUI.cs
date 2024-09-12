using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	private Inventory inventory;
	public static InventoryUI instanceUI;

	public GameObject WeaponSlotsParent;
	public GameObject ApperanceSlotsParent;
	public GameObject PotionSlotsParent;
	public GameObject FoodSlotsParent;
	public GameObject IngridiensSlotsParent;
	public GameObject KeySlotsParent;
	public GameObject HotbarSlotsParent;

	private InventorySlot[] WeaponSlots;
	private InventorySlot[] ApperanceSlots;
	private InventorySlot[] PotionSlots;
	private InventorySlot[] FoodSlots;
	private InventorySlot[] IngridiensSlots;
	private InventorySlot[] KeySlots;
	private InventorySlot[] HotbarSlots;

	public TextMeshProUGUI[] WeaponAmountTexts;
	public TextMeshProUGUI[] ItemApperanceAmountTexts;
	public TextMeshProUGUI[] ItemPotionAmountTexts;
	public TextMeshProUGUI[] ItemFoodAmountTexts;
	public TextMeshProUGUI[] ItemIngridienAmountTexts;
	public TextMeshProUGUI[] KeyAmountTexts;
	public TextMeshProUGUI[] HotbarAmountTexts;

	public TextMeshProUGUI[] CurrentAmountTexts;
	public InventorySlot[] CurrentSlots;
	public int CurrentInventoryCount;

	[Header("Cursor Settings")]
	public RectTransform CursorUI;
	public InventorySlot CursorSlot;

	public bool isAdding = true;

	void Start()
	{
		if (instanceUI != null)
		{
			Debug.Log("Error: More than one instance of Inventory found");
			return;
		}
		instanceUI = this;

		inventory = Inventory.Instance;
		inventory.onItemChangedCallback += UpdateInventoryUI;
		inventory.onMouseItemChangedCallback += UpdateMouseIcon;

		WeaponSlots = WeaponSlotsParent.GetComponentsInChildren<InventorySlot>();
		ApperanceSlots = ApperanceSlotsParent.GetComponentsInChildren<InventorySlot>();
		PotionSlots = PotionSlotsParent.GetComponentsInChildren<InventorySlot>();
		FoodSlots = FoodSlotsParent.GetComponentsInChildren<InventorySlot>();
		IngridiensSlots = IngridiensSlotsParent.GetComponentsInChildren<InventorySlot>();
		KeySlots = KeySlotsParent.GetComponentsInChildren<InventorySlot>();

		WeaponAmountTexts = WeaponSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
		ItemApperanceAmountTexts = ApperanceSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
		ItemPotionAmountTexts = PotionSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
		ItemFoodAmountTexts = FoodSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
		ItemIngridienAmountTexts = IngridiensSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
		KeyAmountTexts = KeySlotsParent.GetComponentsInChildren<TextMeshProUGUI>();

		HotbarSlots = HotbarSlotsParent.GetComponentsInChildren<InventorySlot>();
		HotbarAmountTexts = HotbarSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
	}

	private void Update()
	{
		ControllMouse();
	}

	void UpdateInventoryUI(int slotIndex, bool isAdding)
	{
		ChooseCurentSlotsType();

		if (slotIndex >= 0 && slotIndex < CurrentSlots.Length)
		{
			// Ensure that inventory.listOfItems has enough elements for the given slotIndex

			if (slotIndex < inventory.DictionaryOfItems.Count || isAdding)
			{
				// Check if the item in CurrentSlots matches an item in inventory.listOfItems or if there's an item in the cursor
				if (inventory.DictionaryOfItems.ContainsKey(slotIndex) /*(!inventory.listOfItems.Contains(CurrentSlots[slotIndex].item) && CursorSlot.item != null)*/)
				{
					// Update the specific slot with the item from the inventory list
					CurrentSlots[slotIndex].AddItem(inventory.DictionaryOfItems[slotIndex]);
					CurrentAmountTexts[slotIndex].enabled = inventory.DictionaryOfItems[slotIndex].itemAmount > 1;
				}
				else if (!inventory.DictionaryOfItems.ContainsKey(slotIndex) && CursorSlot.item == null)
				{
					// Clear the slot if no matching item is found and cursor is empty
					CurrentSlots[slotIndex].ClearSlot();
					CurrentAmountTexts[slotIndex].enabled = false;
				}
			}
			else
			{
				// Clear the slot if slotIndex exceeds the list size
				CurrentSlots[slotIndex].ClearSlot();
				if (slotIndex < CurrentAmountTexts.Length)
				{
					CurrentAmountTexts[slotIndex].enabled = false;
				}
			}
		}
		else
		{
			// If the slotIndex is invalid, update all slots
			for (var i = 0; i < CurrentSlots.Length; i++)
			{
				if (i < CurrentInventoryCount)
				{
					CurrentSlots[i].AddItem(inventory.DictionaryOfItems[i]);
					CurrentAmountTexts[i].enabled = inventory.DictionaryOfItems[i].itemAmount > 1;
				}
				else
				{
					CurrentSlots[i].ClearSlot();
					CurrentAmountTexts[i].enabled = false;
				}
			}
		}

		// Update the hotbar UI separately
		UpdateHotbarUI();
	}



	private void UpdateHotbarUI()
	{
		for (int i = 0; i < HotbarSlots.Length; i++)
		{
			if (i < inventory.listOfHotBarItems.Count)
			{
				HotbarSlots[i].AddItem(inventory.listOfHotBarItems[i]);
				HotbarAmountTexts[i].enabled = inventory.listOfHotBarItems[i].itemAmount > 1;
			}
			else
			{
				HotbarSlots[i].ClearSlot();
				HotbarAmountTexts[i].enabled = false;
			}
		}
	}

	private void UpdateMouseIcon()
	{
		if (inventory.MouseItem != null)
		{
			CursorSlot.AddItem(inventory.MouseItem);
		}
		else
		{
			CursorSlot.ClearSlot();
		}
	}

	private void ControllMouse()
	{
		// Pobierz pozycjê myszy w ekranie
		Vector3 mousePosition = Input.mousePosition;

		// Ustawienie pozycji UI na podstawie pozycji myszy
		CursorUI.position = mousePosition;
	}

	private void ChooseCurentSlotsType()
	{
		// Determine the current slot and amount text arrays based on the item type
		switch (inventory.currentItemType.ItemType)
		{
			case ItemTypes.Weapon:
				CurrentSlots = WeaponSlots;
				CurrentAmountTexts = WeaponAmountTexts;
				CurrentInventoryCount = inventory.weaponItems.Count;
				break;
			case ItemTypes.Apperance:
				CurrentSlots = ApperanceSlots;
				CurrentAmountTexts = ItemApperanceAmountTexts;
				CurrentInventoryCount = inventory.apparelItems.Count;
				break;
			case ItemTypes.Potion:
				CurrentSlots = PotionSlots;
				CurrentAmountTexts = ItemPotionAmountTexts;
				CurrentInventoryCount = inventory.potionItems.Count;
				break;
			case ItemTypes.Food:
				CurrentSlots = FoodSlots;
				CurrentAmountTexts = ItemFoodAmountTexts;
				CurrentInventoryCount = inventory.foodItems.Count;
				break;
			case ItemTypes.Ingridiens:
				CurrentSlots = IngridiensSlots;
				CurrentAmountTexts = ItemIngridienAmountTexts;
				CurrentInventoryCount = inventory.ingredientsItems.Count;
				break;
			case ItemTypes.Key:
				CurrentSlots = KeySlots;
				CurrentAmountTexts = KeyAmountTexts;
				CurrentInventoryCount = inventory.keyItems.Count;
				break;
			default:
				CurrentSlots = HotbarSlots;
				CurrentAmountTexts = HotbarAmountTexts;
				CurrentInventoryCount = inventory.listOfHotBarItems.Count;
				break;
		}
	}
}

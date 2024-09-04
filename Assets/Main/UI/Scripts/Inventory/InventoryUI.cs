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

	private TextMeshProUGUI[] WeaponAmountTexts;
	private TextMeshProUGUI[] ItemApperanceAmountTexts;
	private TextMeshProUGUI[] ItemPotionAmountTexts;
	private TextMeshProUGUI[] ItemFoodAmountTexts;
	private TextMeshProUGUI[] ItemIngridienAmountTexts;
	private TextMeshProUGUI[] KeyAmountTexts;
	private TextMeshProUGUI[] HotbarAmountTexts;

	public TextMeshProUGUI[] CurrentAmountTexts;
	public InventorySlot[] CurrentSlots;
	public int CurrentInventoryCount;

	[Header("Cursor Settings")]
	public RectTransform cursorRectTransform;
	public InventorySlot CursorSlot;

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

	void UpdateInventoryUI(int slotIndex)
	{
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
				CurrentInventoryCount = inventory.apperanceItems.Count;
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
				CurrentInventoryCount = inventory.ingridiensItems.Count;
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

		if (slotIndex >= 0 && slotIndex < CurrentSlots.Length)
		{
			CurrentSlots[slotIndex].AddItem(inventory.listOfItems[slotIndex]);
			CurrentAmountTexts[slotIndex].enabled = inventory.listOfItems[slotIndex].itemAmount > 1;
		}
		else
		{
			for (var i = 0; i < CurrentSlots.Length; i++)
			{
				if (i < CurrentInventoryCount)
				{
					CurrentSlots[i].AddItem(inventory.listOfItems[i]);
					CurrentAmountTexts[i].enabled = inventory.listOfItems[i].itemAmount > 1;
				}
				else
				{
					CurrentSlots[i].ClearSlot();
					CurrentAmountTexts[i].enabled = false;
				}
			}
		}

		// Aktualizacja slotów hotbara
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
		Vector2 cursorPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			cursorRectTransform.parent as RectTransform,
			Input.mousePosition,
			Camera.main,
			out cursorPosition);

		// Zaktualizuj pozycjê RectTransform
		cursorRectTransform.anchoredPosition = cursorPosition;
	}
}

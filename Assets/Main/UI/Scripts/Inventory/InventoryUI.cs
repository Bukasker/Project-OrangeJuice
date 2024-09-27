using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	private Inventory inventory;
	public static InventoryUI instanceUI;

	public GameObject SlotsParent;
	public GameObject HotbarSlotsParent;

	private InventorySlot[] Slots;
	private InventorySlot[] HotbarSlots;

	public TextMeshProUGUI[] ItemAmountTexts;
	public TextMeshProUGUI[] HotbarAmountTexts;

	public InventorySlot[] CurrentSlots;
	public TextMeshProUGUI[] CurrentAmountTexts;

	[Header("Cursor Settings")]
	public RectTransform CursorUI;
	public GameObject CursorItem;
	public InventorySlot CursorSlot;
	public TextMeshProUGUI CursorAmountTexts;

	void Start()
	{
		if (instanceUI != null)
		{
			Debug.Log("Error: More than one instance of InventoryUI found");
			return;
		}
		instanceUI = this;

		inventory = Inventory.Instance;
		inventory.onItemChangedCallback += UpdateInventoryUI;
		inventory.onHotBarItemChangedCallback += UpdateHotbarUI;
		inventory.onMouseItemChangedCallback += UpdateMouseIcon;

		// Pobieramy sloty z jednego g³ównego rodzica
		Slots = SlotsParent.GetComponentsInChildren<InventorySlot>();
		ItemAmountTexts = SlotsParent.GetComponentsInChildren<TextMeshProUGUI>();

		// Sloty z Hotbara
		HotbarSlots = HotbarSlotsParent.GetComponentsInChildren<InventorySlot>();
		HotbarAmountTexts = HotbarSlotsParent.GetComponentsInChildren<TextMeshProUGUI>();
	}

	private void Update()
	{
		ControllMouse();
	}

	void UpdateInventoryUI(int slotIndex, bool isAdding)
	{
		if (slotIndex >= 0 && slotIndex < Slots.Length)
		{
			if (slotIndex < inventory.DictionaryOfItems.Count || isAdding)
			{
				if (inventory.DictionaryOfItems.ContainsKey(slotIndex))
				{
					Slots[slotIndex].AddItem(inventory.DictionaryOfItems[slotIndex]);
					ItemAmountTexts[slotIndex].enabled = inventory.DictionaryOfItems[slotIndex].itemAmount > 1;
				}
				else
				{
					Slots[slotIndex].ClearSlot();
					ItemAmountTexts[slotIndex].enabled = false;
				}
			}
			else
			{
				Slots[slotIndex].ClearSlot();
				if (slotIndex < ItemAmountTexts.Length)
				{
					ItemAmountTexts[slotIndex].enabled = false;
				}
			}
		}
	}

	private void UpdateHotbarUI(int slotIndex, bool isAdding)
	{
		if (slotIndex >= 0 && slotIndex < HotbarSlots.Length)
		{
			if (slotIndex < inventory.DictionaryOfHotBarItems.Count || isAdding)
			{
				if (inventory.DictionaryOfHotBarItems.ContainsKey(slotIndex))
				{
					HotbarSlots[slotIndex].AddItem(inventory.DictionaryOfHotBarItems[slotIndex]);
					HotbarAmountTexts[slotIndex].enabled = inventory.DictionaryOfHotBarItems[slotIndex].itemAmount > 1;
				}
				else
				{
					HotbarSlots[slotIndex].ClearSlot();
					HotbarAmountTexts[slotIndex].enabled = false;
				}
			}
			else
			{
				HotbarSlots[slotIndex].ClearSlot();
				if (slotIndex < HotbarAmountTexts.Length)
				{
					HotbarAmountTexts[slotIndex].enabled = false;
				}
			}
		}
	}

	private void UpdateMouseIcon()
	{
		if (inventory.MouseItem != null)
		{
			CursorSlot.AddItem(inventory.MouseItem);
			if (CursorSlot.item.itemAmount > 1)
			{
				CursorAmountTexts.enabled = true;
			}
			else
			{
				CursorAmountTexts.enabled = false;
			}
		}
		else
		{
			CursorSlot.ClearSlot();
		}
	}

	private void ControllMouse()
	{
		if (CursorSlot.item != null)
		{
			CursorItem.SetActive(true);
			Vector3 mousePosition = Input.mousePosition;
			CursorUI.position = mousePosition;
		}
		else
		{
			CursorItem.SetActive(false);
		}
	}
}

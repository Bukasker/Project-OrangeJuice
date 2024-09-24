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
	public InventorySlot CursorSlot;

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

		// Aktualizacja UI Hotbara
		UpdateHotbarUI();
	}

	private void UpdateHotbarUI()
	{
		for (int i = 0; i < HotbarSlots.Length; i++)
		{
			if (i < inventory.DictionaryOfHotBarItems.Count)
			{
				HotbarSlots[i].AddItem(inventory.DictionaryOfHotBarItems[i]);
				HotbarAmountTexts[i].enabled = inventory.DictionaryOfHotBarItems[i].itemAmount > 1;
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
}

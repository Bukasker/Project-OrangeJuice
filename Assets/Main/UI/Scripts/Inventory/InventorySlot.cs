using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour
{
	[SerializeField] private Image icon;
	public Item item = null;
	public int SlotIndex;
	public bool isHotBarSlot;
	public TextMeshProUGUI itemAmoutText;

	public void AddItem(Item newItem)
	{
		item = newItem;
		icon.sprite = item.Icon;
		icon.enabled = true;
		itemAmoutText.enabled = true;
		itemAmoutText.text = Convert.ToString(item.itemAmount);
	}

	public void ClearSlot()
	{
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		itemAmoutText.enabled = false;
	}

	public void OnSlotLeftClicked()
	{
		Inventory.Instance.HandleSlotLeftClicked(item, SlotIndex, isHotBarSlot);
	}
	public void OnSlotRightClicked()
	{
		Inventory.Instance.HandleSlotRightClicked(item, SlotIndex, isHotBarSlot);
	}
	public void OnSlotLeftClickedWithShift()
	{
		Inventory.Instance.HandleSlotLeftClickedWithShift(item, SlotIndex, isHotBarSlot);
	}
	public void OnSlotRightClickedWithShift()
	{
		Inventory.Instance.HandleSlotRightClickedWithShift(item, SlotIndex, isHotBarSlot);
	}
}

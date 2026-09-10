using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    public Item item = null;
    public int SlotIndex;
    public bool isFunctionalSlot;
    public TextMeshProUGUI itemAmoutText;

    void Awake()
    {
        SlotIndex = transform.GetSiblingIndex();
    }
    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.Icon;
        icon.enabled = true;
        itemAmoutText.enabled = true;
        itemAmoutText.text = Convert.ToString(item.ItemAmount);
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
        Inventory.Instance.HandleSlotLeftClicked(item, SlotIndex, isFunctionalSlot);
    }
    public void OnSlotRightClicked()
    {
        Inventory.Instance.HandleSlotRightClicked(item, SlotIndex, isFunctionalSlot);
    }
    public void OnSlotLeftClickedWithShift()
    {
        Inventory.Instance.HandleSlotLeftClickedWithShift(item, SlotIndex, isFunctionalSlot);
    }
    public void OnSlotRightClickedWithShift()
    {
        Inventory.Instance.HandleSlotRightClickedWithShift(item, SlotIndex, isFunctionalSlot);
    }
}

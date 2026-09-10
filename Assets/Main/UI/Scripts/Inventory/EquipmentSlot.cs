using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Sprite oryginalIcon;
    public Item item = null;
    public int SlotIndex;
    public TextMeshProUGUI itemAmoutText;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.Icon;

        if (itemAmoutText != null)
        {
            itemAmoutText.enabled = true;
            itemAmoutText.text = Convert.ToString(item.ItemAmount);
        }
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = oryginalIcon;

        if (itemAmoutText != null)
        {
            itemAmoutText.enabled = false;
        }
    }

    public void OnSlotLeftClicked()
    {
        EquipmentManager.Instance.HandleSlotLeftClickedEquip(item, SlotIndex);
    }
    public void OnSlotRightClicked()
    {
        EquipmentManager.Instance.HandleSlotLeftClickedEquip(item, SlotIndex);
    }
    public void OnSlotLeftClickedWithShift()
    {
        EquipmentManager.Instance.UnequipToClosestSlotInInventory(item, SlotIndex);
    }
    public void OnSlotRightClickedWithShift()
    {
        EquipmentManager.Instance.UnequipToClosestSlotInInventory(item, SlotIndex);
    }

}

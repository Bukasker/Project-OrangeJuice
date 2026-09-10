using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton

    public static EquipmentManager Instance;

    public delegate void OnEquipmentChanged(Item newItem, Item oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<EquipmentSlot> EquipmentSlots;
    public List<Item> currentEquipment;

    public PlayerAnimationsController animator;

    private ItemType currentItemType;
    private WeaponType currentWeaponType;
    private ArmorType currentArmorType;
    private int choosedSlotIndex;

    [SerializeField] private SpriteRenderer WeaponRenderer;
    [SerializeField] private PlayerLayersSync PantsSync;
    [SerializeField] private PlayerLayersSync TunicSync;
    [SerializeField] private PlayerLayersSync BeltSync;
    [SerializeField] private PlayerLayersSync BootsSync;
    [SerializeField] private PlayerLayersSync GreavesSync;
    [SerializeField] private PlayerLayersSync BracketsSync;
    [SerializeField] private PlayerLayersSync HelmetSync;

    private PlayerLayersSync currentLayer;

    public bool IsSwordEquiped;
    public bool IsBowEquiped;
    public bool IsSpellEquiped;

    private Item lastEquipedItem;


    public void HandleSelectedItemChangeInHotBar(InventorySlot slot)
    {
        var slotItem = slot.item;

        if (slotItem != null)
        {
            if (slotItem.ItemType == ItemType.Weapon)
            {
                animator.swordEquiped = true;
                IsSwordEquiped = true;
                WeaponRenderer.sprite = slotItem.WeaponSpriteSheet;
                lastEquipedItem = slotItem;
                onEquipmentChanged?.Invoke(slotItem, null);
                return;
            }
        }
        onEquipmentChanged?.Invoke(null, lastEquipedItem);
        lastEquipedItem = null;
        animator.swordEquiped = false;
        IsSwordEquiped = false;
        WeaponRenderer.sprite = null;
    }

    public void HandleSlotLeftClickedEquip(Item newItem, int slotIndex)
    {
        var mouseItem = Inventory.Instance.MouseItem;

        if (mouseItem != null)
        {
            choosedSlotIndex = ChooseEquipSlot(mouseItem);

            if (choosedSlotIndex == slotIndex)
            {
                if (newItem != null && mouseItem.ItemName == newItem.ItemName)
                {
                    int totalAmount = newItem.ItemAmount + mouseItem.ItemAmount;

                    if (totalAmount <= newItem.MaxStack)
                    {
                        newItem.ItemAmount = totalAmount;
                        Inventory.Instance.RemoveItemFromMouse();
                    }
                    else
                    {
                        newItem.ItemAmount = newItem.MaxStack;
                        mouseItem.ItemAmount = totalAmount - newItem.MaxStack;
                    }

                    EquipmentSlots[slotIndex].AddItem(newItem);
                    Inventory.Instance.onMouseItemChangedCallback?.Invoke();
                    return;
                }

                if (newItem == null)
                {
                    EquipmentSlots[slotIndex].AddItem(mouseItem);
                    onEquipmentChanged?.Invoke(mouseItem, null);

                    Inventory.Instance.RemoveItemFromMouse();
                    Inventory.Instance.onMouseItemChangedCallback?.Invoke();

                    // ZMIANA ARMORU
                    UpdateArmorVisual(mouseItem);
                    return;
                }

                EquipmentSlots[slotIndex].AddItem(mouseItem);
                onEquipmentChanged?.Invoke(mouseItem, newItem);

                Inventory.Instance.AddItemToMouse(newItem, slotIndex, false);
                Inventory.Instance.onMouseItemChangedCallback?.Invoke();

                // ZMIANA ARMORU
                UpdateArmorVisual(mouseItem);
                return;
            }
            else
            {
                Inventory.Instance.AddItemToMouse(newItem, slotIndex, false);
                EquipmentSlots[slotIndex].ClearSlot();
                onEquipmentChanged?.Invoke(null, newItem);

                Inventory.Instance.onMouseItemChangedCallback?.Invoke();

                // OPCJONALNE CZYSZCZENIE WARSTWY
                ClearArmorVisual(newItem);
                return;
            }
        }
        else
        {
            if (newItem != null)
            {
                Inventory.Instance.AddItemToMouse(newItem, slotIndex, false);
                EquipmentSlots[slotIndex].ClearSlot();
                onEquipmentChanged?.Invoke(null, newItem);

                Inventory.Instance.onMouseItemChangedCallback?.Invoke();

                // OPCJONALNE CZYSZCZENIE WARSTWY
                ClearArmorVisual(newItem);
            }
        }
    }

    public void HandleSlotLeftClickedWithShiftEquip(Item newItem, int slotIndex, bool isHotBarSlot)
    {
        if (newItem == null) return;

        choosedSlotIndex = ChooseEquipSlot(newItem);
        Item oldItem = currentEquipment[choosedSlotIndex];
        bool alreadyEquipped = currentEquipment.Contains(newItem);

        if (oldItem != null && oldItem != newItem)
        {
            Inventory.Instance.AddItemToClosestSlot(Instantiate(oldItem), false);
        }

        Item itemToEquip = Instantiate(newItem);
        EquipmentSlots[choosedSlotIndex].AddItem(itemToEquip);
        currentEquipment[choosedSlotIndex] = itemToEquip;

        if (!alreadyEquipped)
        {
            Inventory.Instance.RemoveItemFromSlot(slotIndex, isHotBarSlot);
        }
        else
        {
            EquipmentSlots[slotIndex].ClearSlot();
            currentEquipment[slotIndex] = null;
        }

        onEquipmentChanged?.Invoke(itemToEquip, oldItem);

        // ZMIANA ARMORU
        UpdateArmorVisual(itemToEquip);

        choosedSlotIndex = -1;
    }

    public void UnequipToClosestSlotInInventory(Item newItem, int slotIndex)
    {
        Item oldItem = currentEquipment[slotIndex];
        if (oldItem != null)
        {
            Inventory.Instance.AddItemToClosestSlot(Instantiate(oldItem), false);
            EquipmentSlots[slotIndex].ClearSlot();
            currentEquipment[slotIndex] = null;

            onEquipmentChanged?.Invoke(null, oldItem);
            choosedSlotIndex = -1;

            // OPCJONALNE CZYSZCZENIE WARSTWY
            ClearArmorVisual(oldItem);
        }
    }

    private int ChooseEquipSlot(Item item)
    {
        var index = -1;
        currentItemType = item.ItemType;
        currentArmorType = item.ArmorType;

        if (currentItemType == ItemType.Apperance)
        {
            if (item.ArmorType == ArmorType.Ring && EquipmentSlots[4].item != null)
            {
                index = 5;
                return index;
            }

            switch (currentArmorType)
            {
                case ArmorType.Helmet: index = 0; break;
                case ArmorType.Armor: index = 1; break;
                case ArmorType.Pants: index = 2; break;
                case ArmorType.Boots: index = 3; break;
                case ArmorType.Ring: index = 4; break;
                case ArmorType.Belt: index = 6; break;
                case ArmorType.Greaves: index = 7; break;
                case ArmorType.Glove: index = 8; break;
            }
        }
        else if (currentItemType == ItemType.Potion || currentItemType == ItemType.Food)
        {
            index = 9;
        }
        return index;
    }

    private PlayerLayersSync ChooseItemType(Item item)
    {
        currentItemType = item.ItemType;
        currentWeaponType = item.WeaponType;
        currentArmorType = item.ArmorType;

        if (currentArmorType != ArmorType.None)
        {
            switch (currentArmorType)
            {
                case ArmorType.Helmet: currentLayer = HelmetSync; break;
                case ArmorType.Armor: currentLayer = TunicSync; break;
                case ArmorType.Pants: currentLayer = PantsSync; break;
                case ArmorType.Boots: currentLayer = BootsSync; break;
                case ArmorType.Belt: currentLayer = BeltSync; break;
                case ArmorType.Greaves: currentLayer = GreavesSync; break;
                case ArmorType.Glove: currentLayer = BracketsSync; break;
            }
        }

        return currentLayer;
    }

    private void UpdateArmorVisual(Item item)
    {
        if (item.ItemType == ItemType.Apperance && item.ArrmorSpriteSheet != null)
        {
            var layer = ChooseItemType(item);
            layer.LoadNewLayerSheet(item.ArrmorSpriteSheet);
        }
    }

    private void ClearArmorVisual(Item item)
    {
        if (item.ItemType == ItemType.Apperance)
        {
            var layer = ChooseItemType(item);
            layer.LoadNewLayerSheet(null);
        }
    }
}

using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<int, Item> DictionaryOfItems = new Dictionary<int, Item>();
    public Dictionary<int, Item> DictionaryOfHotBarItems = new Dictionary<int, Item>();

    public Item MouseItem;
    public Item currentItemType;

    public delegate void OnItemChange(int slotIndex, bool isAdding);
    public OnItemChange onItemChangedCallback;

    public delegate void OnHotBarItemChange(int slotIndex, bool isAdding);
    public OnHotBarItemChange onHotBarItemChangedCallback;

    public delegate void OnMouseItemChange();
    public OnMouseItemChange onMouseItemChangedCallback;

    public static Inventory Instance;

    public bool lastSlotWasHotBar;
    public int lastSlotIndex;




    #region Singleton
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Error: More than one instance of Inventory found");
            return;
        }
        Instance = this;
    }
    #endregion

    public void AddItemToMouse(Item item, int slotIndex, bool isHotBarSlot)
    {
        MouseItem = item;
        lastSlotIndex = slotIndex;
        lastSlotWasHotBar = isHotBarSlot;
        onMouseItemChangedCallback?.Invoke();
    }

    public void RemoveItemFromMouse()
    {
        MouseItem = null;
        onMouseItemChangedCallback?.Invoke();
    }

    public void AddItemToClosestSlot(Item item, bool isHotBarSlot)
    {
        var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;
        int remainingAmount = item.ItemAmount;

        // 1. Spróbuj dodaæ do istniej¹cych stacków
        foreach (var entry in dictionaryToUse)
        {
            Item inventoryItem = entry.Value;

            if (inventoryItem != null && inventoryItem.ItemName == item.ItemName && inventoryItem.ItemAmount < inventoryItem.MaxStack)
            {
                int spaceLeft = inventoryItem.MaxStack - inventoryItem.ItemAmount;
                int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);

                inventoryItem.ItemAmount += amountToAdd;
                remainingAmount -= amountToAdd;

                if (isHotBarSlot)
                    onHotBarItemChangedCallback?.Invoke(entry.Key, true);
                else
                    onItemChangedCallback?.Invoke(entry.Key, true);

                if (remainingAmount <= 0)
                    return; // wszystko dodano
            }
        }

        // 2. Dodaj nowe stacki do wolnych slotów
        while (remainingAmount > 0)
        {
            int slotIndex = -1;

            // ZnajdŸ pierwszy wolny slot
            for (int i = 0; i < 21; i++) // Upewnij siê, ¿e 21 to odpowiednia liczba slotów
            {
                if (!dictionaryToUse.ContainsKey(i) || dictionaryToUse[i] == null)
                {
                    slotIndex = i;
                    break;
                }
            }

            if (slotIndex == -1)
            {
                Debug.LogWarning("Brak wolnych slotów w ekwipunku!");
                return; // brak miejsca na resztê itemów
            }

            Item newItem = Instantiate(item);
            newItem.ItemAmount = Mathf.Min(remainingAmount, newItem.MaxStack);
            remainingAmount -= newItem.ItemAmount;

            dictionaryToUse[slotIndex] = newItem;

            if (isHotBarSlot)
            {
                onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                
            }
            else
            {
                onItemChangedCallback?.Invoke(slotIndex, true);
            }
        }
    }


    public void AddItemToSlot(Item item, int slotIndex, bool isHotBarSlot)
    {
        var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;

        if (dictionaryToUse.ContainsKey(slotIndex) && dictionaryToUse[slotIndex] != null)
        {
            Item existingItem = dictionaryToUse[slotIndex];

            if (existingItem.ItemName == item.ItemName)
            {
                int potentialNewAmount = existingItem.ItemAmount + item.ItemAmount;
                if (potentialNewAmount <= existingItem.MaxStack)
                {
                    existingItem.ItemAmount += item.ItemAmount;

                    if (isHotBarSlot)
                    {
                        onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                    }
                    else
                    {
                        onItemChangedCallback?.Invoke(slotIndex, true);
                    }
                    return;
                }
            }
        }

        dictionaryToUse[slotIndex] = Instantiate(item);

        if (isHotBarSlot)
        {
            onHotBarItemChangedCallback?.Invoke(slotIndex, true);
        }
        else
        {
            onItemChangedCallback?.Invoke(slotIndex, true);
        }
    }

    public void RemoveItemFromSlot(int slotIndex, bool isHotBarSlot)
    {
        var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;

        if (dictionaryToUse.ContainsKey(slotIndex))
        {
            dictionaryToUse.Remove(slotIndex);

            if (isHotBarSlot)
            {
                onHotBarItemChangedCallback?.Invoke(slotIndex, false);
            }
            else
            {
                onItemChangedCallback?.Invoke(slotIndex, false);
            }
        }
    }

    public void ReturnItemFromMouseToLastPosition()
    {
        if (MouseItem != null)
        {
            var copyItem = Instantiate(MouseItem);
            AddItemToSlot(copyItem, lastSlotIndex, lastSlotWasHotBar);
            RemoveItemFromMouse();
            onMouseItemChangedCallback?.Invoke();
        }
    }

    public void HandleSlotRightClicked(Item item, int slotIndex, bool isHotBarSlot)
    {

        var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;

        if (MouseItem != null)
        {
            // Je¿eli klikniêto prawym przyciskiem myszy, a gracz ma przedmiot na myszy, zdejmij jedn¹ sztukê i przenieœ j¹ do slotu
            if (item != null)
            {
                if (MouseItem.ItemName == item.ItemName && MouseItem.ItemAmount >= 1)
                {
                    var tempItemAmount = item.ItemAmount + MouseItem.ItemAmount;
                    if (tempItemAmount <= item.MaxStack)
                    {
                        MouseItem.ItemAmount--;
                        item.ItemAmount++;

                        if (MouseItem.ItemAmount < 1)
                        {
                            RemoveItemFromMouse();
                        }

                    }
                    if (isHotBarSlot)
                    {
                        onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                    }
                    else
                    {
                        onItemChangedCallback?.Invoke(slotIndex, true);
                    }

                    onMouseItemChangedCallback?.Invoke();
                    return;
                }
                else
                {
                    var tempItem = item;
                    AddItemToSlot(MouseItem, slotIndex, isHotBarSlot);
                    AddItemToMouse(tempItem, slotIndex, isHotBarSlot);
                    if (isHotBarSlot)
                    {
                        onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                    }
                    else
                    {
                        onItemChangedCallback?.Invoke(slotIndex, true);
                    }
                    onMouseItemChangedCallback?.Invoke();
                    return;
                }
            }
            else if (MouseItem != null && item == null && MouseItem.ItemAmount >= 1)
            {
                // Jeœli gracz klikn¹³ prawym przyciskiem myszy na pusty slot, przenieœ pojedyncz¹ sztukê z przedmiotu na myszy do tego slotu
                MouseItem.ItemAmount--;
                var copyItem = Instantiate(MouseItem);
                copyItem.ItemAmount = 1;
                AddItemToSlot(copyItem, slotIndex, isHotBarSlot);

                if (MouseItem.ItemAmount <= 0)
                {
                    RemoveItemFromMouse();
                }
                if (isHotBarSlot)
                {
                    onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                }
                else
                {
                    onItemChangedCallback?.Invoke(slotIndex, true);
                }

                onMouseItemChangedCallback?.Invoke();
                return;
            }

        }
        else if (item != null)
        {
            // Je¿eli gracz nie ma przedmiotu na myszy, zdejmij jedn¹ sztukê z wybranego slotu i przenieœ j¹ na mysz
            MouseItem = Instantiate(item);
            MouseItem.ItemAmount = 1;
            item.ItemAmount--;

            if (item.ItemAmount <= 0)
            {
                RemoveItemFromSlot(slotIndex, isHotBarSlot);
                if (isHotBarSlot)
                {
                    onHotBarItemChangedCallback?.Invoke(slotIndex, false);
                }
                else
                {
                    onItemChangedCallback?.Invoke(slotIndex, false);
                }
            }
            if (isHotBarSlot)
            {
                onHotBarItemChangedCallback?.Invoke(slotIndex, true);
            }
            else
            {
                onItemChangedCallback?.Invoke(slotIndex, true);
            }
            onMouseItemChangedCallback?.Invoke();
            return;
        }
    }

    public void HandleSlotLeftClicked(Item item, int slotIndex, bool isHotBarSlot)
    {
        var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;

        if (MouseItem != null)
        {
            if (item != null && MouseItem.ItemName == item.ItemName && MouseItem.ItemAmount >= 1)
            {
                var tempItemAmount = item.ItemAmount + MouseItem.ItemAmount;
                if (tempItemAmount <= item.MaxStack)
                {
                    item.ItemAmount += MouseItem.ItemAmount;
                    RemoveItemFromMouse();
                }
                else if (item.ItemAmount++ <= item.MaxStack)
                {
                    var overItemAmount = tempItemAmount - item.MaxStack;
                    item.ItemAmount = item.MaxStack;
                    MouseItem.ItemAmount = overItemAmount;
                    onMouseItemChangedCallback?.Invoke();
                }

                if (isHotBarSlot)
                {
                    onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                }
                else
                {
                    onItemChangedCallback?.Invoke(slotIndex, true);
                }

                onMouseItemChangedCallback?.Invoke();
                return;
            }
            
            else if (MouseItem != null && item == null)
            {
                AddItemToSlot(MouseItem, slotIndex, isHotBarSlot);
                RemoveItemFromMouse();

                onMouseItemChangedCallback?.Invoke();
                return;
            }
            else
            {
                var tempItem = item;
                AddItemToSlot(MouseItem, slotIndex, isHotBarSlot);
                AddItemToMouse(tempItem, slotIndex, isHotBarSlot);
                if (isHotBarSlot)
                {
                    onHotBarItemChangedCallback?.Invoke(slotIndex, true);
                }
                else
                {
                    onItemChangedCallback?.Invoke(slotIndex, true);
                }
                onMouseItemChangedCallback?.Invoke();
                return;
            }
        }
        else
        {
            RemoveItemFromSlot(slotIndex, isHotBarSlot);
            AddItemToMouse(item, slotIndex, isHotBarSlot);
            if (isHotBarSlot)
            {
                onHotBarItemChangedCallback?.Invoke(slotIndex, true);
            }
            else
            {
                onItemChangedCallback?.Invoke(slotIndex, true);
            }
            onMouseItemChangedCallback?.Invoke();
            return;
        }
    }

    public void HandleSlotLeftClickedWithShift(Item item, int slotIndex, bool isHotBarSlot)
    {
        if (item == null) return;

        if (item.ItemType == ItemType.Apperance || item.ItemType == ItemType.Potion || item.ItemType == ItemType.Food)
        {
            EquipmentManager.Instance.HandleSlotLeftClickedWithShiftEquip(item, slotIndex, isHotBarSlot);
            if (isHotBarSlot)
            {
                RemoveItemFromSlot(slotIndex, true);
            }
            else
            {
                RemoveItemFromSlot(slotIndex, false);
            }
        }
        else
        {
            var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;
            var copyItem = Instantiate(item);
            if (item != null)
            {
                if (isHotBarSlot)
                {
                    AddItemToClosestSlot(copyItem, false);
                    RemoveItemFromSlot(slotIndex, true);
                }
                else
                {
                    AddItemToClosestSlot(copyItem, true);
                    RemoveItemFromSlot(slotIndex, false);
                }
            }
        }
    }

    public void HandleSlotRightClickedWithShift(Item item, int slotIndex, bool isHotBarSlot)
    {
        if (item == null) return;

        if (item.ItemType == ItemType.Apperance || item.ItemType == ItemType.Potion || item.ItemType == ItemType.Food)
        {
            EquipmentManager.Instance.HandleSlotLeftClickedWithShiftEquip(item, slotIndex, isHotBarSlot);
            if (isHotBarSlot)
            {
                RemoveItemFromSlot(slotIndex, true);
            }
            else
            {
                RemoveItemFromSlot(slotIndex, false);
            }
        }
        else
        {
            var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;
            if (item != null)
            {
                var copyItem = Instantiate(item);
                copyItem.ItemAmount = 1;
                item.ItemAmount--;

                if (isHotBarSlot)
                {
                    AddItemToSlot(copyItem, slotIndex, false);
                    if (item.ItemAmount <= 0)
                    {
                        RemoveItemFromSlot(slotIndex, true);
                    }
                }
                else
                {
                    AddItemToSlot(copyItem, slotIndex, true);
                    if (item.ItemAmount <= 0)
                    {
                        RemoveItemFromSlot(slotIndex, false);
                    }
                }
            }
        }
    }
    public void ReturnItemFromMouseToLastPositionSafe()
    {
        if (MouseItem == null) return;

        var dictionaryToUse = lastSlotWasHotBar ? DictionaryOfHotBarItems : DictionaryOfItems;

        // Czy slot istnieje?
        if (dictionaryToUse.TryGetValue(lastSlotIndex, out Item existingItem))
        {
            // Jeœli slot zawiera ten sam typ przedmiotu, spróbuj zestackowaæ
            if (existingItem != null && existingItem.ItemName == MouseItem.ItemName)
            {
                int spaceLeft = existingItem.MaxStack - existingItem.ItemAmount;
                int amountToAdd = Mathf.Min(spaceLeft, MouseItem.ItemAmount);

                existingItem.ItemAmount += amountToAdd;
                MouseItem.ItemAmount -= amountToAdd;

                if (MouseItem.ItemAmount <= 0)
                {
                    RemoveItemFromMouse();
                }

                if (lastSlotWasHotBar)
                    onHotBarItemChangedCallback?.Invoke(lastSlotIndex, true);
                else
                    onItemChangedCallback?.Invoke(lastSlotIndex, true);

                onMouseItemChangedCallback?.Invoke();
                return;
            }
            else
            {

                Debug.LogWarning("Nie mo¿na zwróciæ przedmiotu do poprzedniego slotu – slot zajêty innym przedmiotem.");
                return;
            }
        }

        // Slot by³ pusty, wiêc mo¿na zwróciæ przedmiot
        AddItemToSlot(Instantiate(MouseItem), lastSlotIndex, lastSlotWasHotBar);
        RemoveItemFromMouse();

        if (lastSlotWasHotBar)
            onHotBarItemChangedCallback?.Invoke(lastSlotIndex, true);
        else
            onItemChangedCallback?.Invoke(lastSlotIndex, true);

        onMouseItemChangedCallback?.Invoke();
    }

}
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public List<Item> weaponItems = new List<Item>();
	public List<Item> apperanceItems = new List<Item>();
	public List<Item> potionItems = new List<Item>();
	public List<Item> foodItems = new List<Item>();
	public List<Item> ingridiensItems = new List<Item>();
	public List<Item> keyItems = new List<Item>();

	public delegate void OnItemChange(int slotIndex);
	public OnItemChange onItemChangedCallback;

	public delegate void OnMouseItemChange();
	public OnMouseItemChange onMouseItemChangedCallback;

	public static Inventory Instance;

	public List<Item> listOfItems = new List<Item>();
	public List<Item> listOfHotBarItems = new List<Item>();
	public Item MouseItem;
	public Item currentItemType;

	#region Singleton
	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("Error : More than one instance of Inventory found");
			return;
		}
		Instance = this;
	}
	#endregion

	public void AddItemToMouse(Item item, int slotIndex)
	{
		MouseItem = item;
		onMouseItemChangedCallback?.Invoke();
		RemoveItemFromSlot(item, slotIndex);
		onItemChangedCallback?.Invoke(slotIndex);
	}

	public void RemoveItemFromMouse()
	{
		MouseItem = null;
		onMouseItemChangedCallback?.Invoke();
	}

	public void AddItemToHotBar(Item item, int slotIndex)
	{
		if (slotIndex >= 0 && slotIndex < listOfHotBarItems.Count)
		{
			listOfHotBarItems[slotIndex] = item;
		}
		else
		{
			listOfHotBarItems.Add(item);
		}
		onItemChangedCallback?.Invoke(slotIndex);
	}

	public void RemoveItemFromHotBar(Item item, int slotIndex)
	{
		listOfHotBarItems.RemoveAt(slotIndex);
		onItemChangedCallback?.Invoke(slotIndex);
	}

	public void AddItemToClosestSlot(Item item)
	{
		currentItemType = item;
		ChooseItemList(item);
		bool itemAlreadyInInventory = false;
		foreach (Item inventoryItem in listOfItems)
		{
			var allItemAmount = inventoryItem.itemAmount + item.itemAmount;
			if (inventoryItem.ItemName == item.ItemName && (allItemAmount) <= inventoryItem.maxStack)
			{
				inventoryItem.itemAmount += item.itemAmount;
				itemAlreadyInInventory = true;
			}
		}
		if (!itemAlreadyInInventory)
		{
			if (item != null)
			{
				Item copyItem = Instantiate(item);
				listOfItems.Add(copyItem);
			}
		}
		var slotIndex = -1;
		onItemChangedCallback?.Invoke(slotIndex);
	}

	public void AddItemToSlot(Item item, int slotIndex)
	{
		currentItemType = item;
		ChooseItemList(item);
		bool itemAlreadyInInventory = false;
		foreach (Item inventoryItem in listOfItems)
		{
			var allItemAmount = inventoryItem.itemAmount + item.itemAmount;
			if (inventoryItem.ItemName == item.ItemName && (allItemAmount) <= inventoryItem.maxStack)
			{
				inventoryItem.itemAmount += item.itemAmount;
				itemAlreadyInInventory = true;
			}
		}
		if (!itemAlreadyInInventory)
		{
			if (item != null)
			{
				Item copyItem = Instantiate(item);
				listOfItems.Add(copyItem);
			}
		}
		onItemChangedCallback?.Invoke(slotIndex);
	}

	public void RemoveItemFromSlot(Item item, int slotIndex)
	{
		currentItemType = item;
		ChooseItemList(item);
		listOfItems.Remove(item);
		onItemChangedCallback?.Invoke(slotIndex);
	}

	public void ChooseItemList(Item item)
	{
		if (item != null)
		{
			switch (item.ItemType)
			{
				case ItemTypes.Weapon:
					listOfItems = weaponItems;
					break;
				case ItemTypes.Apperance:
					listOfItems = apperanceItems;
					break;
				case ItemTypes.Potion:
					listOfItems = potionItems;
					break;
				case ItemTypes.Food:
					listOfItems = foodItems;
					break;
				case ItemTypes.Ingridiens:
					listOfItems = ingridiensItems;
					break;
				case ItemTypes.Key:
					listOfItems = keyItems;
					break;
			}
		}
	}

	public void HandleSlotClick(Item item, int slotIndex)
	{
		MouseItem = item;
		if (MouseItem != null)
		{
			AddItemToSlot(MouseItem, slotIndex);
			RemoveItemFromMouse(); 
		}
		else
		{
			AddItemToMouse(item, slotIndex);
		}
	}
}

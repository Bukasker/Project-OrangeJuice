using System.Collections.Generic;
using UnityEditor.VersionControl;
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

	public void AddItemToMouse(Item item, int slotIndex,bool isHotBarSlot)
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
		currentItemType = item;
		bool itemAlreadyInInventory = false;

		var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;

		foreach (var entry in dictionaryToUse)
		{
			Item inventoryItem = entry.Value;

			if (inventoryItem != null && inventoryItem.ItemName == item.ItemName)
			{
				int potentialNewAmount = inventoryItem.itemAmount + item.itemAmount;
				if (potentialNewAmount <= inventoryItem.maxStack)
				{
					inventoryItem.itemAmount += item.itemAmount;
					itemAlreadyInInventory = true;

					if (isHotBarSlot)
					{
						onHotBarItemChangedCallback?.Invoke(entry.Key, true);
					}
					else
					{
						onItemChangedCallback?.Invoke(entry.Key, true);
					}
					break;
				}
			}
		}

		if (!itemAlreadyInInventory)
		{
			int slotIndex = -1;
			for (int i = 0; i < 21; i++)
			{
				if (!dictionaryToUse.ContainsKey(i) || dictionaryToUse[i] == null)
				{
					slotIndex = i;
					break;
				}
			}

			if (slotIndex != -1)
			{
				Item newItem = Instantiate(item);
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
			else
			{
				Debug.LogWarning("Brak wolnych slotów w ekwipunku!");
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
				int potentialNewAmount = existingItem.itemAmount + item.itemAmount;
				if (potentialNewAmount <= existingItem.maxStack)
				{
					existingItem.itemAmount += item.itemAmount;

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
				if (MouseItem.ItemName == item.ItemName && MouseItem.itemAmount >= 1)
				{
					MouseItem.itemAmount--;
					item.itemAmount++;

					if (MouseItem.itemAmount < 1)
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
			else if (MouseItem != null && item == null && MouseItem.itemAmount >= 1)
			{
				// Jeœli gracz klikn¹³ prawym przyciskiem myszy na pusty slot, przenieœ pojedyncz¹ sztukê z przedmiotu na myszy do tego slotu
				MouseItem.itemAmount--;
				var copyItem = Instantiate(MouseItem);
				copyItem.itemAmount = 1;
				AddItemToSlot(copyItem, slotIndex, isHotBarSlot);

				if (MouseItem.itemAmount <= 0)
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
			MouseItem.itemAmount = 1;
			item.itemAmount--;

			if (item.itemAmount <= 0)
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
			if (MouseItem.ItemName == item?.ItemName && item.itemAmount < item.maxStack)
			{
				item.itemAmount += MouseItem.itemAmount;
				RemoveItemFromMouse();

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
		var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;
		var copyItem = Instantiate(item);
		if (item != null)
		{
			if (isHotBarSlot)
			{
				AddItemToClosestSlot(copyItem, false);
				onItemChangedCallback?.Invoke(slotIndex, true);
				RemoveItemFromSlot(slotIndex, true);
				onHotBarItemChangedCallback?.Invoke(slotIndex, false);
			}
			else
			{
				AddItemToClosestSlot(copyItem, true);
				onHotBarItemChangedCallback?.Invoke(slotIndex, true);
				RemoveItemFromSlot(slotIndex, false);
				onItemChangedCallback?.Invoke(slotIndex, false);
			}
		}
	}

	public void HandleSlotRightClickedWithShift(Item item, int slotIndex, bool isHotBarSlot)
	{
		var dictionaryToUse = isHotBarSlot ? DictionaryOfHotBarItems : DictionaryOfItems;

		if (item != null)
		{
			var copyItem = Instantiate(item);
			copyItem.itemAmount = 1;
			item.itemAmount--;

			if (isHotBarSlot)
			{
				AddItemToSlot(copyItem, slotIndex, false);
				onItemChangedCallback?.Invoke(slotIndex, true);
				if (item.itemAmount <= 0)
				{
					RemoveItemFromSlot(slotIndex, true);
				}
				onHotBarItemChangedCallback?.Invoke(slotIndex, false);
			}
			else
			{
				AddItemToSlot(copyItem, slotIndex, true);
				onHotBarItemChangedCallback?.Invoke(slotIndex, true);
				if (item.itemAmount <= 0)
				{
					RemoveItemFromSlot(slotIndex, false);
				}
				onItemChangedCallback?.Invoke(slotIndex, false);
			}
		}
	}


}
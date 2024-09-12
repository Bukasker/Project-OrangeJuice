using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	// S³owniki zamiast list
	public Dictionary<int, Item> weaponItems = new Dictionary<int, Item>();
	public Dictionary<int, Item> apparelItems = new Dictionary<int, Item>();
	public Dictionary<int, Item> potionItems = new Dictionary<int, Item>();
	public Dictionary<int, Item> foodItems = new Dictionary<int, Item>();
	public Dictionary<int, Item> ingredientsItems = new Dictionary<int, Item>();
	public Dictionary<int, Item> keyItems = new Dictionary<int, Item>();

	public Dictionary<int, Item> DictionaryOfItems = new Dictionary<int, Item>();
	public Dictionary<int, Item> listOfHotBarItems = new Dictionary<int, Item>();

	public Item MouseItem;
	public Item currentItemType;

	// Modyfikujemy callback tak, aby przekazywa³ dodatkowy parametr `isAdding`
	public delegate void OnItemChange(int slotIndex, bool isAdding);
	public OnItemChange onItemChangedCallback;

	public delegate void OnMouseItemChange();
	public OnMouseItemChange onMouseItemChangedCallback;

	public static Inventory Instance;

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

	public void AddItemToMouse(Item item, int slotIndex)
	{
		MouseItem = item;
		onMouseItemChangedCallback?.Invoke();
	}

	public void RemoveItemFromMouse()
	{
		MouseItem = null;
		onMouseItemChangedCallback?.Invoke();
	}

	public void AddItemToHotBar(Item item, int slotIndex)
	{
		listOfHotBarItems[slotIndex] = item;
		onItemChangedCallback?.Invoke(slotIndex, true);  // Dodajemy przedmiot
	}

	public void RemoveItemFromHotBar(int slotIndex)
	{
		if (listOfHotBarItems.ContainsKey(slotIndex))
		{
			listOfHotBarItems.Remove(slotIndex);
			onItemChangedCallback?.Invoke(slotIndex, false);  // Usuwamy przedmiot
		}
	}

	public void AddItemToClosestSlot(Item item)
	{
		currentItemType = item;
		ChooseItemList(item);

		bool itemAlreadyInInventory = false;
		foreach (var entry in DictionaryOfItems)
		{
			Item inventoryItem = entry.Value;

			if (inventoryItem != null && inventoryItem.ItemName == item.ItemName)
			{
				int potentialNewAmount = inventoryItem.itemAmount + item.itemAmount;
				if (potentialNewAmount <= inventoryItem.maxStack)
				{
					inventoryItem.itemAmount += item.itemAmount;
					itemAlreadyInInventory = true;
					onItemChangedCallback?.Invoke(entry.Key, true);  // Dodajemy przedmiot
					break;
				}
			}
		}

		if (!itemAlreadyInInventory)
		{
			int slotIndex = -1;
			for (int i = 0; i < 21; i++)
			{
				if (!DictionaryOfItems.ContainsKey(i) || DictionaryOfItems[i] == null)
				{
					slotIndex = i;
					break;
				}
			}

			if (slotIndex != -1)
			{
				Item newItem = Instantiate(item);
				DictionaryOfItems[slotIndex] = newItem;
				onItemChangedCallback?.Invoke(slotIndex, true);  // Dodajemy przedmiot
			}
			else
			{
				Debug.LogWarning("Brak wolnych slotów w ekwipunku!");
			}
		}
	}

	public void AddItemToSlot(Item item, int slotIndex)
	{
		currentItemType = item;
		ChooseItemList(item);

		if (DictionaryOfItems.ContainsKey(slotIndex) && DictionaryOfItems[slotIndex] != null)
		{
			Item existingItem = DictionaryOfItems[slotIndex];

			if (existingItem.ItemName == item.ItemName)
			{
				int potentialNewAmount = existingItem.itemAmount + item.itemAmount;
				if (potentialNewAmount <= existingItem.maxStack)
				{
					existingItem.itemAmount += item.itemAmount;
					onItemChangedCallback?.Invoke(slotIndex, true);  // Dodajemy przedmiot
					return;
				}
			}

			Debug.LogWarning("Slot zajêty przez inny przedmiot. Zastêpowanie przedmiotu.");
		}

		if (item != null)
		{
			Item newItem = Instantiate(item);
			DictionaryOfItems[slotIndex] = newItem;
		}

		onItemChangedCallback?.Invoke(slotIndex, true);  // Dodajemy przedmiot
	}

	public void RemoveItemFromSlot(int slotIndex)
	{
		if (DictionaryOfItems.ContainsKey(slotIndex))
		{
			DictionaryOfItems.Remove(slotIndex);
			onItemChangedCallback?.Invoke(slotIndex, false);  // Usuwamy przedmiot
		}
	}
	public void HandleSlotRightClicked(Item item, int slotIndex)
	{
		if (MouseItem != null)
		{
			// Je¿eli klikniêto prawym przyciskiem myszy, a gracz ma przedmiot na myszy, zdejmij jedn¹ sztukê i przenieœ j¹ do slotu
			if (item != null)
			{
				if (MouseItem.ItemName == item.ItemName && MouseItem.itemAmount >= 1)
				{
					MouseItem.itemAmount--;
					item.itemAmount++;

					if (MouseItem.itemAmount <= 0)
					{
						RemoveItemFromMouse();
						onItemChangedCallback?.Invoke(slotIndex, true);
						onMouseItemChangedCallback?.Invoke();
						return;
					}

					onItemChangedCallback?.Invoke(slotIndex, true);

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
				AddItemToSlot(copyItem, slotIndex);
				onItemChangedCallback?.Invoke(slotIndex, true);

				if (MouseItem.itemAmount <= 0)
				{
					RemoveItemFromMouse();
					onItemChangedCallback?.Invoke(slotIndex, true);
					onMouseItemChangedCallback?.Invoke();
					return;
				}
				onItemChangedCallback?.Invoke(slotIndex, true);

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
				RemoveItemFromSlot(slotIndex);
				onItemChangedCallback?.Invoke(slotIndex, false);
				onMouseItemChangedCallback?.Invoke();
				return;
			}


			onItemChangedCallback?.Invoke(slotIndex, true);
			onMouseItemChangedCallback?.Invoke();

			return;

		}
	}

	public void HandleSlotLeftClicked(Item item, int slotIndex)
	{
		if (MouseItem != null)
		{
			// Je¿eli gracz ma przedmiot na myszy i klika na slot z tym samym przedmiotem, dodaj do stosu
			if (MouseItem.ItemName == item?.ItemName && item.itemAmount < item.maxStack)
			{
				item.itemAmount = item.itemAmount + MouseItem.itemAmount;
				RemoveItemFromMouse();
				onItemChangedCallback?.Invoke(slotIndex, true);
				onMouseItemChangedCallback?.Invoke();
				return;

			}
			else if (MouseItem != null && item == null)
			{
				// Je¿eli gracz ma przedmiot w myszce i klika na pusty slot
				AddItemToSlot(MouseItem, slotIndex);
				RemoveItemFromMouse();
				onItemChangedCallback?.Invoke(slotIndex, true);
				onMouseItemChangedCallback?.Invoke();
				return;
			}
			else
			{
				// Je¿eli gracz klika na inny przedmiot, zamieñ przedmioty
				var tempItem = item;
				AddItemToMouse(tempItem, slotIndex);
				onItemChangedCallback?.Invoke(slotIndex, true);
				onMouseItemChangedCallback?.Invoke();
				return;
			}
		}
		else
		{
			// Je¿eli gracz nie ma przedmiotu na myszy, przenieœ przedmiot do myszy
			RemoveItemFromSlot(slotIndex);
			AddItemToMouse(item, slotIndex);
			onItemChangedCallback?.Invoke(slotIndex, false);
			onMouseItemChangedCallback?.Invoke();
			return;
		}

	}

	public void HandleSlotLeftClickedWithShift(Item item, int slotIndex)
	{
		// Natychmiastowe przeniesienie przedmiotu do najbli¿szego wolnego slotu
		if (item != null)
		{
			AddItemToClosestSlot(item);
			RemoveItemFromSlot(slotIndex);
		}

		onItemChangedCallback?.Invoke(slotIndex, true);
	}

	public void HandleSlotRightClickedWithShift(Item item, int slotIndex)
	{
		// Obs³uga prawokliku + Shift (mo¿na dodaæ dodatkowe funkcjonalnoœci w zale¿noœci od potrzeb)
		// W tym przyk³adzie zak³adam, ¿e mo¿e przenieœæ wszystkie sztuki przedmiotu do najbli¿szego wolnego slotu.
		if (item != null && MouseItem == null)
		{
			AddItemToClosestSlot(item);
			RemoveItemFromSlot(slotIndex);
		}

		onItemChangedCallback?.Invoke(slotIndex, true);
	}

	public void ChooseItemList(Item item)
	{
		if (item != null)
		{
			switch (item.ItemType)
			{
				case ItemTypes.Weapon:
					DictionaryOfItems = weaponItems;
					break;
				case ItemTypes.Apperance:
					DictionaryOfItems = apparelItems;
					break;
				case ItemTypes.Potion:
					DictionaryOfItems = potionItems;
					break;
				case ItemTypes.Food:
					DictionaryOfItems = foodItems;
					break;
				case ItemTypes.Ingridiens:
					DictionaryOfItems = ingredientsItems;
					break;
				case ItemTypes.Key:
					DictionaryOfItems = keyItems;
					break;
			}
		}
	}
}
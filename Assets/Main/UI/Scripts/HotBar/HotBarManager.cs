using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HotBarManager : MonoBehaviour
{
	public List<SlotBackgroundChange> slotsBackgrounds;
	public List<InventorySlot> slots;
	public InventorySlot choosedSlot;
	private int selectedIndex = 0;

	public UnityEvent<InventorySlot> ChangeChoosedInventorySlot;

	void Start()
	{
		if (slotsBackgrounds.Count > 0 && slots.Count > 0)
		{
			slotsBackgrounds[selectedIndex].Select();
			choosedSlot = slots[selectedIndex];
		}
	}

	void Update()
	{
		HandleScrollInput();
		HandleNumberInput();
	}

	private void HandleScrollInput()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll > 0f)
		{
			SelectNextSlot();
		}
		else if (scroll < 0f)
		{
			SelectPreviousSlot();
		}
	}

	private void HandleNumberInput()
	{
		// Obs³uga przycisków od 1 do =
		if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
		if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
		if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
		if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
		if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
		if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
		if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
		if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
		if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);
		if (Input.GetKeyDown(KeyCode.Alpha0)) SelectSlot(9);
		if (Input.GetKeyDown(KeyCode.Minus)) SelectSlot(10);
		if (Input.GetKeyDown(KeyCode.Equals)) SelectSlot(11);
	}

	private void SelectNextSlot()
	{
		int nextIndex = (selectedIndex + 1) % slotsBackgrounds.Count;
		SelectSlot(nextIndex);
	}

	private void SelectPreviousSlot()
	{
		int prevIndex = (selectedIndex - 1 + slotsBackgrounds.Count) % slotsBackgrounds.Count;
		SelectSlot(prevIndex);
	}

	private void SelectSlot(int index)
	{
		if (index >= 0 && index < slotsBackgrounds.Count && index != selectedIndex)
		{
			slotsBackgrounds[selectedIndex].Deselect();
			selectedIndex = index;
			slotsBackgrounds[selectedIndex].Select();
			choosedSlot = slots[selectedIndex];

			if (choosedSlot != null)
			{
				ChangeChoosedInventorySlot.Invoke(choosedSlot);
			}
		}
	}

	public void RefreshSlots(int slotIndex)
    {
		if(slotIndex == selectedIndex)
		{
            ChangeChoosedInventorySlot.Invoke(slots[slotIndex]);
        }
    }
}

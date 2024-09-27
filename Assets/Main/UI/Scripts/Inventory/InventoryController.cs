using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour
{
	[Header("Inventory Settings")]
	[SerializeField] private GameObject inventory;
	[SerializeField] private KeyCode invetoryKey = KeyCode.I;
	[SerializeField] private KeyCode invetoryAlternativeKey = KeyCode.Tab;
	[SerializeField] private UnityEvent removeItemFromMouse;

	[Header("Category Settings")]
	[SerializeField] private GameObject[] categorySlots;
	private GameObject activeCategory;

	[Header("Animation Settings")]
	[SerializeField] private RectTransform inventoryTransform;
	[SerializeField] float animationStartValue = 0f;
	[SerializeField] float animationEndValue = 400f;
	[SerializeField] float animationOpenDuration = 0.75f;
	[SerializeField] float animationCloseDuration = 1.5f;
	[SerializeField] private bool inventoryIsActive = false;

	private Tween currentTween;

	private void Update()
	{
		if (Input.GetKeyDown(invetoryKey) || Input.GetKeyDown(invetoryAlternativeKey))
		{
			inventoryIsActive = !inventoryIsActive;

			if (inventoryIsActive)
			{
				Open();
			}
			else
			{
				removeItemFromMouse.Invoke();
				Close();
			}
		}
	}

	private void Open()
	{
		if (currentTween != null && currentTween.IsPlaying())
		{
			currentTween.Kill();
		}

		inventory.SetActive(true);
		currentTween = inventoryTransform.DOMoveY(animationEndValue, animationOpenDuration).OnComplete(() =>
		{

		});
	}

	private void Close()
	{
		if (currentTween != null && currentTween.IsPlaying())
		{
			currentTween.Kill();
		}

		currentTween = inventoryTransform.DOMoveY(animationStartValue, animationCloseDuration).OnComplete(() =>
		{
			if (!inventoryIsActive)
			{
				inventory.SetActive(false);
			}
		});
	}
	public void SetCategoryActive(int CategoryIndex)
	{
		for (int i = 0; i < categorySlots.Length; i++)
		{
			categorySlots[i].SetActive(false);
		}
		categorySlots[CategoryIndex].SetActive(true);
		activeCategory = categorySlots[CategoryIndex];
	}
}

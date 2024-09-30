using UnityEngine;

public class InventoryCategoryManager : MonoBehaviour
{
    [SerializeField] private GameObject[] categorySlots;
	private GameObject activeCategory;

    private void SetCategoryActive(int CategoryIndex)
    {
		for (int i = 0; i < categorySlots.Length; i++)
		{
			categorySlots[i].SetActive(false);
		}
		categorySlots[CategoryIndex].SetActive(true);
		activeCategory = categorySlots[CategoryIndex];
	}
}

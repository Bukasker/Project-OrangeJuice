using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSummaryMenager : MonoBehaviour
{
	[SerializeField] private GameObject summaryObject;
	[SerializeField] private GameObject slotPrefab;
	[SerializeField] private GameObject plusPrefab;

	public void DisplayIngridience(int numberOfIngridience)
	{
		if (numberOfIngridience >= 2)
		{
			summaryObject.SetActive(true);
			Instantiate(slotPrefab);
			Instantiate(plusPrefab);
			Instantiate(slotPrefab);
			if(numberOfIngridience > 2)
			{
				for (int i = 2; i < numberOfIngridience; i++)
				{
					Instantiate(plusPrefab);
					Instantiate(slotPrefab);
				}
			}
		}
		else if (numberOfIngridience == 1)
		{
			summaryObject.SetActive(true);

		}
		else
		{
			summaryObject.SetActive(false);
		}
	}
}

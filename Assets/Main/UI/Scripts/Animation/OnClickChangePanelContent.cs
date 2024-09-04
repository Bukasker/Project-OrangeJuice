using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickChangePanelContent : MonoBehaviour
{
	[SerializeField] private GameObject[] gameObjectslist;

	public void OnClick(GameObject panelToTurnOn)
	{
		for (int i = 0; i < gameObjectslist.Length; i++)
		{
			gameObjectslist[i].SetActive(false);
		}
		panelToTurnOn.SetActive(true);
	}
}

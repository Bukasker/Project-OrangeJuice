using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnActiveMoveToSide : MonoBehaviour
{
	[SerializeField] private RectTransform[] buttonsTransform;
	[SerializeField] private float xStartPosition;
	[SerializeField] private float offsetNumber;

	// Indeks guzika, kt�ry ma by� aktywny na pocz�tku
	[SerializeField] private int defaultActiveButtonIndex = 0;

	// Flaga okre�laj�ca, czy sekcja jest aktywna
	public static bool isSectionActive = false;

	// Zmienna do przechowywania ostatnio aktywnego przycisku
	private int lastActiveButtonIndex = -1;

	private void Start()
	{
		// Ustaw domy�lny aktywny guzik przy starcie
		SetButtonTransformX(defaultActiveButtonIndex);
	}

	public void SetButtonTransformX(int buttonIndex)
	{
		isSectionActive = true;  // Aktywuj sekcj�, gdy guziki s� przesuwane

		// Zresetuj stan ostatniego aktywnego guzika
		if (lastActiveButtonIndex != -1 && lastActiveButtonIndex < buttonsTransform.Length)
		{
			ChangeButtonOnHover lastButtonHoverScript = buttonsTransform[lastActiveButtonIndex].GetComponent<ChangeButtonOnHover>();
			if (lastButtonHoverScript != null)
			{
				lastButtonHoverScript.ResetPressedState();
			}
		}

		// Przesu� wybrany guzik o offset, aby by� aktywny
		for (int i = 0; i < buttonsTransform.Length; i++)
		{
			Vector2 newPosition = buttonsTransform[i].anchoredPosition;
			newPosition.x = xStartPosition;
			buttonsTransform[i].anchoredPosition = newPosition;
		}

		buttonsTransform[buttonIndex].anchoredPosition = new Vector2(buttonsTransform[buttonIndex].anchoredPosition.x - offsetNumber,
																	 buttonsTransform[buttonIndex].anchoredPosition.y);

		// Ustaw stan aktywnego przycisku
		ChangeButtonOnHover currentButtonHoverScript = buttonsTransform[buttonIndex].GetComponent<ChangeButtonOnHover>();
		if (currentButtonHoverScript != null)
		{
			currentButtonHoverScript.SetPressedState();
		}

		// Zaktualizuj indeks ostatnio aktywnego guzika
		lastActiveButtonIndex = buttonIndex;
	}

	// Metoda, aby wy��czy� aktywno�� sekcji
	public void DeactivateSection()
	{
		isSectionActive = false;
	}
}

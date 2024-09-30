using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnActiveMoveToSide : MonoBehaviour
{
	[SerializeField] private RectTransform[] buttonsTransform;
	[SerializeField] private float xStartPosition;
	[SerializeField] private float offsetNumber;

	// Indeks guzika, który ma byæ aktywny na pocz¹tku
	[SerializeField] private int defaultActiveButtonIndex = 0;

	// Flaga okreœlaj¹ca, czy sekcja jest aktywna
	public static bool isSectionActive = false;

	// Zmienna do przechowywania ostatnio aktywnego przycisku
	private int lastActiveButtonIndex = -1;

	private void Start()
	{
		// Ustaw domyœlny aktywny guzik przy starcie
		SetButtonTransformX(defaultActiveButtonIndex);
	}

	public void SetButtonTransformX(int buttonIndex)
	{
		isSectionActive = true;  // Aktywuj sekcjê, gdy guziki s¹ przesuwane

		// Zresetuj stan ostatniego aktywnego guzika
		if (lastActiveButtonIndex != -1 && lastActiveButtonIndex < buttonsTransform.Length)
		{
			ChangeButtonOnHover lastButtonHoverScript = buttonsTransform[lastActiveButtonIndex].GetComponent<ChangeButtonOnHover>();
			if (lastButtonHoverScript != null)
			{
				lastButtonHoverScript.ResetPressedState();
			}
		}

		// Przesuñ wybrany guzik o offset, aby by³ aktywny
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

	// Metoda, aby wy³¹czyæ aktywnoœæ sekcji
	public void DeactivateSection()
	{
		isSectionActive = false;
	}
}

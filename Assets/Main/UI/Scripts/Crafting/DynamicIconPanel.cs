using UnityEngine;
using UnityEngine.UI;

public class DynamicIconPanel : MonoBehaviour
{
	public RectTransform panel;     // Panel do modyfikacji
	public GameObject iconPrefab;   // Prefab ikony do dodania
	public float iconHeight = 100f; // Wysokoœæ ikony (mo¿na pobraæ dynamicznie)
	public float offset = 10f;      // Odstêp miêdzy ikonami

	// Funkcja dodaj¹ca ikonê do panelu i zwiêkszaj¹ca wysokoœæ panelu
	public void AddIconToPanel()
	{
		// Stwórz now¹ ikonê na podstawie prefab'u
		GameObject newIcon = Instantiate(iconPrefab, panel);

		// Pobierz aktualn¹ wysokoœæ panelu
		float currentHeight = panel.sizeDelta.y;

		// Oblicz now¹ wysokoœæ panelu: aktualna wysokoœæ + wysokoœæ ikony + offset
		float newHeight = currentHeight + iconHeight + offset;

		// Ustaw now¹ wysokoœæ panelu
		panel.sizeDelta = new Vector2(panel.sizeDelta.x, newHeight);

		// Ustaw ikonê w nowej pozycji (w pionie)
		newIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(currentHeight + offset));
	}
}

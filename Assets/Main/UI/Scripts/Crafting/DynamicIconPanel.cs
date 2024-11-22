using UnityEngine;
using UnityEngine.UI;

public class DynamicIconPanel : MonoBehaviour
{
	public RectTransform panel;     // Panel do modyfikacji
	public GameObject iconPrefab;   // Prefab ikony do dodania
	public float iconHeight = 100f; // Wysoko�� ikony (mo�na pobra� dynamicznie)
	public float offset = 10f;      // Odst�p mi�dzy ikonami

	// Funkcja dodaj�ca ikon� do panelu i zwi�kszaj�ca wysoko�� panelu
	public void AddIconToPanel()
	{
		// Stw�rz now� ikon� na podstawie prefab'u
		GameObject newIcon = Instantiate(iconPrefab, panel);

		// Pobierz aktualn� wysoko�� panelu
		float currentHeight = panel.sizeDelta.y;

		// Oblicz now� wysoko�� panelu: aktualna wysoko�� + wysoko�� ikony + offset
		float newHeight = currentHeight + iconHeight + offset;

		// Ustaw now� wysoko�� panelu
		panel.sizeDelta = new Vector2(panel.sizeDelta.x, newHeight);

		// Ustaw ikon� w nowej pozycji (w pionie)
		newIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(currentHeight + offset));
	}
}

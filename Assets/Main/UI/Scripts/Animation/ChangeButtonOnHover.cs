using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeButtonOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Image Settings")]
	public Image panelImage;
	public Sprite hoveredSprite;
	public Sprite originalSprite;

	// Flaga oznaczaj�ca, czy guzik jest aktywny (wci�ni�ty)
	public bool isPressed = false;

	private bool isAlreadyHovered;
	private int originalSiblingIndex;

	public void OnPointerEnter(PointerEventData eventData)
	{
		// Ignoruj najechanie myszk�, je�li guzik jest wci�ni�ty
		if (isPressed)
			return;

		originalSiblingIndex = transform.GetSiblingIndex();
		panelImage.sprite = hoveredSprite;
		transform.SetAsLastSibling();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		// Ignoruj opuszczenie myszki, je�li guzik jest wci�ni�ty
		if (isPressed)
			return;

		panelImage.sprite = originalSprite;
		transform.SetSiblingIndex(originalSiblingIndex);
	}

	// Ustaw guzik jako aktywny (wci�ni�ty)
	public void SetPressedState()
	{
		isPressed = true;
		panelImage.sprite = hoveredSprite; // Zmieniamy grafik� na aktywn�
	}

	// Resetowanie stanu wci�ni�cia guzika
	public void ResetPressedState()
	{
		isPressed = false;
		panelImage.sprite = originalSprite; // Zmieniamy grafik� na domy�ln�
	}
}

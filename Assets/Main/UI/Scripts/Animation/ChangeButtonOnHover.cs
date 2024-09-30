using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeButtonOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Image Settings")]
	public Image panelImage;
	public Sprite hoveredSprite;
	public Sprite originalSprite;

	// Flaga oznaczaj¹ca, czy guzik jest aktywny (wciœniêty)
	public bool isPressed = false;

	private bool isAlreadyHovered;
	private int originalSiblingIndex;

	public void OnPointerEnter(PointerEventData eventData)
	{
		// Ignoruj najechanie myszk¹, jeœli guzik jest wciœniêty
		if (isPressed)
			return;

		originalSiblingIndex = transform.GetSiblingIndex();
		panelImage.sprite = hoveredSprite;
		transform.SetAsLastSibling();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		// Ignoruj opuszczenie myszki, jeœli guzik jest wciœniêty
		if (isPressed)
			return;

		panelImage.sprite = originalSprite;
		transform.SetSiblingIndex(originalSiblingIndex);
	}

	// Ustaw guzik jako aktywny (wciœniêty)
	public void SetPressedState()
	{
		isPressed = true;
		panelImage.sprite = hoveredSprite; // Zmieniamy grafikê na aktywn¹
	}

	// Resetowanie stanu wciœniêcia guzika
	public void ResetPressedState()
	{
		isPressed = false;
		panelImage.sprite = originalSprite; // Zmieniamy grafikê na domyœln¹
	}
}

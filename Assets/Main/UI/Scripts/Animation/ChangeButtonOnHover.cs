using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeButtonOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Image Settings")]
	public Image panelImage;
	public Sprite hoveredSprite;
	public Sprite originalSprite;

	private int originalSiblingIndex;
	public void OnPointerEnter(PointerEventData eventData)
	{
		originalSiblingIndex = transform.GetSiblingIndex();
		panelImage.sprite = hoveredSprite;
		transform.SetAsLastSibling();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		panelImage.sprite = originalSprite;
		transform.SetSiblingIndex(originalSiblingIndex);
	}
}

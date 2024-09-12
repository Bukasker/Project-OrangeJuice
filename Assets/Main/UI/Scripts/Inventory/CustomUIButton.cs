using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CustomUIButton :MonoBehaviour, IPointerClickHandler {

	public UnityEvent leftClick;
	public UnityEvent rightClick;
	public UnityEvent leftClickWithShift;
	public UnityEvent rightClickWithShift;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
			leftClick.Invoke();
		else if (eventData.button == PointerEventData.InputButton.Right)
			rightClick.Invoke();
		else if (eventData.button == PointerEventData.InputButton.Left && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftShift)))
			leftClickWithShift.Invoke();
		else if (eventData.button == PointerEventData.InputButton.Right && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftShift)))
			rightClickWithShift.Invoke();
	}
}

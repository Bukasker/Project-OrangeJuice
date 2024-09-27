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
		if (eventData.button == PointerEventData.InputButton.Left && (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.LeftShift)))
			leftClick.Invoke();
		else if (eventData.button == PointerEventData.InputButton.Right && (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.LeftShift)))
			rightClick.Invoke();
		else if (eventData.button == PointerEventData.InputButton.Left && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)))
			leftClickWithShift.Invoke();
		else if (eventData.button == PointerEventData.InputButton.Right && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)))
			rightClickWithShift.Invoke();
	}
}

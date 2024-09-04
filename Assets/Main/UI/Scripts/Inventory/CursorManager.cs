using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CursorManager : MonoBehaviour
{
	[Header("Cursor Settings")]
	[SerializeField] private Texture2D regularCursor;
	[SerializeField] private Texture2D hoverCursor;
	[SerializeField] private Texture2D clickCursor;
	[SerializeField] private Vector2 cursorHotspot = Vector2.zero;

	public void SetRegularCursor()
	{
		Cursor.SetCursor(regularCursor, cursorHotspot, CursorMode.ForceSoftware);
	}

	public void SetHoverCursor()
	{
		Cursor.SetCursor(hoverCursor, cursorHotspot, CursorMode.ForceSoftware);
	}
	public void SetClickCursor()
	{
		Cursor.SetCursor(clickCursor, cursorHotspot, CursorMode.ForceSoftware);
	}
}

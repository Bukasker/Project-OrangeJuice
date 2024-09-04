using UnityEngine;
using UnityEngine.UI;

public class SlotBackgroundChange : MonoBehaviour
{
	public Image backgroundImage; // T�o slotu
	public Sprite normalBackground; // Domy�lna grafika t�a
	public Sprite selectedBackground; // Grafika t�a, gdy slot jest wybrany

	public void Select()
	{
		backgroundImage.sprite = selectedBackground;
	}

	public void Deselect()
	{
		backgroundImage.sprite = normalBackground;
	}
}

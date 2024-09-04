using UnityEngine;
using UnityEngine.UI;

public class SlotBackgroundChange : MonoBehaviour
{
	public Image backgroundImage; // T³o slotu
	public Sprite normalBackground; // Domyœlna grafika t³a
	public Sprite selectedBackground; // Grafika t³a, gdy slot jest wybrany

	public void Select()
	{
		backgroundImage.sprite = selectedBackground;
	}

	public void Deselect()
	{
		backgroundImage.sprite = normalBackground;
	}
}

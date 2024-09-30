using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnActiveChangeButton : MonoBehaviour
{	
	[SerializeField] private Image[] imageToChange;
	[SerializeField] private Image[] iconToChange;
	[SerializeField] private Sprite[] spriteActive;
	[SerializeField] private Sprite[] spriteDisactive;
	[SerializeField] private Sprite[] iconsActive;
	[SerializeField] private Sprite[] iconsDisactive;

	public void ChangeButtonOnActive(int imageIndex)
	{
		for (int i = 0; i < imageToChange.Length; i++)
		{
			imageToChange[i].sprite = spriteDisactive[i];
		}
		for (int i = 0; i < iconToChange.Length; i++)
		{
			iconToChange[i].sprite = iconsDisactive[i];
		}

		imageToChange[imageIndex].sprite = spriteActive[imageIndex];
		iconToChange[imageIndex].sprite = iconsActive[imageIndex];
	}
}

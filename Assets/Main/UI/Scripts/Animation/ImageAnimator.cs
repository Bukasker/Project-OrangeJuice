using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageAnimator : MonoBehaviour
{
    public Sprite[] Sprites;
    public float FrameRate = 0.2f;
    public Image ImageRenderer;


    void Start()
    {

        if (Sprites.Length > 0 && ImageRenderer != null)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    IEnumerator PlayAnimation()
    {
        int index = 0;

        while (true)
        {
            ImageRenderer.sprite = Sprites[index];
            index = (index + 1) % Sprites.Length;
            yield return new WaitForSeconds(FrameRate);
        }
    }

}

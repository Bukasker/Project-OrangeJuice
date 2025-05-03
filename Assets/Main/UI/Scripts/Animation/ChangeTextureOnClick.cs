using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeTextureOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite[] textures;
    [SerializeField] private Image imageComponent;
    private int currentIndex = 0;

    void Start()
    {
        if (textures.Length > 0 && imageComponent != null)
        {
            imageComponent.sprite = textures[0];
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (textures.Length == 0 || imageComponent == null)
            return;

        if (currentIndex < textures.Length - 1)
        {
            currentIndex++;
            imageComponent.sprite = textures[currentIndex];
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAnimationOnClick : MonoBehaviour, IPointerClickHandler
{
    public Animation animationComponent; // Komponent Animation
    public string clipName;              // Nazwa klipu animacji do odpalenia

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animationComponent != null && animationComponent.GetClip(clipName) != null)
        {
            animationComponent.Play(clipName);
        }
    }
}

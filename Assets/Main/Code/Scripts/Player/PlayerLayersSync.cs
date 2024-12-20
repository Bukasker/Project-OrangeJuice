using UnityEngine;

public class PlayerLayersSync : MonoBehaviour
{
    public SpriteRenderer layerRenderer;
    public Sprite[] layerSpritesUp;    // Sprite'y dla ruchu w g�r�
    public Sprite[] layerSpritesDown;  // Sprite'y dla ruchu w d�
    public Sprite[] layerSpritesLeft;  // Sprite'y dla ruchu w lewo
    public Sprite[] layerSpritesRight; // Sprite'y dla ruchu w prawo

    public Animator bodyAnimator;

    public float X;
    public float Y;

    void LateUpdate()
    {
        // Pobierz warto�ci float�w x i y z Animatora
        X = bodyAnimator.GetFloat("X");
        Y = bodyAnimator.GetFloat("Y");

        // Okre�l kierunek na podstawie warto�ci x i y
        Sprite[] currentArmorSprites;

        if (Mathf.Abs(X) > Mathf.Abs(Y))
        {
            // Ruch w prawo/lewo
            currentArmorSprites = X > 0 ? layerSpritesRight : layerSpritesLeft;
        }
        else
        {
            // Ruch w g�r�/d�
            currentArmorSprites = Y > 0 ? layerSpritesUp : layerSpritesDown;
        }

        // Pobierz bie��cy stan animacji i klatk�
        AnimatorStateInfo stateInfo = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        int frameIndex = Mathf.FloorToInt(stateInfo.normalizedTime * currentArmorSprites.Length) % currentArmorSprites.Length;

        // Ustaw sprite dla aktualnej klatki
        layerRenderer.sprite = currentArmorSprites[frameIndex];
    }
}

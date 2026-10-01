using UnityEngine;

// Simple pixel-style particles; each definition can replace them with its own prefab.
public class StatusEffectVisual : MonoBehaviour
{
    private static Sprite pixel;
    private readonly SpriteRenderer[] motes = new SpriteRenderer[8];
    private StatusEffectType kind;
    private Color tint;
    private float age;

    public static GameObject Create(StatusEffectDefinition effect, Transform parent)
    {
        if (!effect.showVisuals) return null;
        if (effect.visualPrefab != null)
        {
            var custom = Instantiate(effect.visualPrefab, parent);
            custom.transform.localPosition = effect.visualOffset;
            return custom;
        }
        var root = new GameObject(effect.effectType + " VFX");
        root.transform.SetParent(parent, false);
        root.transform.localPosition = effect.visualOffset;
        var visual = root.AddComponent<StatusEffectVisual>();
        visual.kind = effect.effectType;
        visual.tint = effect.visualColor;
        var ownerSprite = parent.GetComponentInChildren<SpriteRenderer>();
        if (pixel == null)
        {
            pixel = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            pixel.name = "Status Effect Pixel";
        }
        for (int i = 0; i < visual.motes.Length; i++)
        {
            var mote = new GameObject("Particle");
            mote.transform.SetParent(root.transform, false);
            var renderer = mote.AddComponent<SpriteRenderer>();
            renderer.sprite = pixel;
            renderer.color = Color.clear;
            if (ownerSprite != null)
            {
                renderer.sortingLayerID = ownerSprite.sortingLayerID;
                renderer.sortingOrder = ownerSprite.sortingOrder + 1;
            }
            visual.motes[i] = renderer;
        }
        return root;
    }

    private void Update()
    {
        age += Time.deltaTime;
        for (int i = 0; i < motes.Length; i++)
        {
            float phase = Mathf.Repeat(age * 1.4f + i / (float)motes.Length, 1f);
            float x = Mathf.Sin(i * 7.3f) * 0.35f;
            float y = phase * 0.7f;
            Vector3 scale = Vector3.one * 0.055f;
            if (kind == StatusEffectType.Bleeding)
            {
                y = -phase * phase;
                scale = new Vector3(0.035f, 0.09f, 1f);
            }
            else if (kind == StatusEffectType.Poison)
            {
                x += Mathf.Sin(phase * 6f + i) * 0.12f;
                scale *= 1f + phase;
            }
            else if (kind == StatusEffectType.Slow)
            {
                x = Mathf.Cos(phase * Mathf.PI * 2f) * 0.4f;
                y = Mathf.Sin(phase * Mathf.PI * 2f) * 0.13f - 0.35f;
            }
            motes[i].transform.localPosition = new Vector3(x, y, 0f);
            motes[i].transform.localScale = scale;
            Color color = tint;
            color.a *= Mathf.Sin(phase * Mathf.PI);
            motes[i].color = color;
        }
    }
}

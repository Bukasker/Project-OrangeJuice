using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[DisallowMultipleComponent]
public class JuicyHealthBar : MonoBehaviour
{
    [Header("Connection")]
    public CharacterStats healthSource;
    [Tooltip("Optional HP counter. Leave empty if your bar has no counter.")]
    public TMP_Text healthText;

    [Header("Smooth drain and damage trail")]
    [Min(0.01f)] public float drainSpeed = 10f;
    [Min(0f)] public float trailDelay = 0.25f;
    [Min(0.01f)] public float trailSpeed = 3f;
    public Color trailColor = new Color(1f, 0.85f, 0.45f);

    [Header("Hit reaction")]
    [Min(0.01f)] public float hitDuration = 0.3f;
    [Min(0f)] public float shakePixels = 4f;
    [Range(0f, 1f)] public float punch = 0.3f;
    public Color hitColor = Color.white;

    [Header("Low health and healing")]
    [Range(0f, 1f)] public float lowHealthThreshold = 0.25f;
    public Color lowHealthColor = new Color(1f, 0.25f, 0.25f);
    public Color healColor = new Color(0.55f, 1f, 0.65f);
    public bool healingSparkles = true;

    [Header("Floating damage numbers")]
    public bool damageNumbers = true;
    public TMP_FontAsset numberFont;
    [Min(1)] public int numberFontSize = 18;
    [Min(0.01f)] public float numberLifetime = 0.7f;
    public float numberRise = 55f;

    private Slider bar;
    private RectTransform barRect;
    private Image fill;
    private Image trail;
    private Vector2 restPosition;
    private Vector3 restScale;
    private Color restColor;
    private float shown;
    private float trailing;
    private float lastHealth;
    private float trailTimer;
    private float hitTimer;
    private float healTimer;
    private float sparkleTimer;
    private bool initialized;
    private readonly List<FloatingGraphic> floating = new List<FloatingGraphic>();

    private class FloatingGraphic
    {
        public Graphic graphic;
        public Vector2 origin;
        public Vector2 velocity;
        public float age;
        public float lifetime;
        public Color color;
    }

    private void Awake()
    {
        bar = GetComponent<Slider>();
        barRect = (RectTransform)transform;
        fill = bar.fillRect != null ? bar.fillRect.GetComponent<Image>() : null;
        if (fill != null) restColor = fill.color;
    }

    private void OnEnable()
    {
        restPosition = barRect.anchoredPosition;
        restScale = barRect.localScale;
        initialized = false;
    }

    // Run after CharacterStats and PlayerStats have assigned the raw slider value.
    private void LateUpdate()
    {
        if (healthSource == null) return;

        float health = Mathf.Clamp(healthSource.currentHealth, healthSource.MinHealth, healthSource.MaxHealth);
        float target = Mathf.InverseLerp(healthSource.MinHealth, healthSource.MaxHealth, health);
        float dt = Time.deltaTime;
        if (!initialized)
        {
            shown = trailing = target;
            lastHealth = health;
            initialized = true;
            CreateTrail();
        }

        if (health < lastHealth)
        {
            trailing = Mathf.Max(trailing, shown);
            trailTimer = trailDelay;
            hitTimer = hitDuration;
            healTimer = 0f;
            if (damageNumbers) SpawnNumber(lastHealth - health);
        }
        else if (health > lastHealth)
        {
            healTimer = hitDuration;
            hitTimer = 0f;
            trailTimer = 0f;
            trailing = target;
        }
        lastHealth = health;

        shown = Mathf.Lerp(shown, target, 1f - Mathf.Exp(-drainSpeed * dt));
        if (Mathf.Abs(shown - target) < 0.0001f) shown = target;
        trailTimer = Mathf.Max(0f, trailTimer - dt);
        if (trailTimer <= 0f)
            trailing = Mathf.Lerp(trailing, target, 1f - Mathf.Exp(-trailSpeed * dt));
        if (Mathf.Abs(trailing - target) < 0.0001f) trailing = target;

        bar.minValue = healthSource.MinHealth;
        bar.maxValue = healthSource.MaxHealth;
        bar.SetValueWithoutNotify(Mathf.Lerp(bar.minValue, bar.maxValue, shown));
        UpdateTrail(Mathf.Max(shown, trailing));

        hitTimer = Mathf.Max(0f, hitTimer - dt);
        healTimer = Mathf.Max(0f, healTimer - dt);
        float hit = hitTimer / Mathf.Max(0.01f, hitDuration);
        float heal = healTimer / Mathf.Max(0.01f, hitDuration);
        float wave = Mathf.Sin((1f - hit) * Mathf.PI * 4f) * hit;
        barRect.anchoredPosition = restPosition + new Vector2(wave, wave * 0.5f) * shakePixels;
        barRect.localScale = Vector3.Scale(restScale, new Vector3(1f, 1f + punch * wave, 1f));
        Color color = restColor;
        if (target > 0f && target <= lowHealthThreshold)
            color = Color.Lerp(restColor, lowHealthColor, 0.55f + 0.45f * Mathf.Sin(Time.time * 8f));
        color = Color.Lerp(color, hitColor, hit);
        color = Color.Lerp(color, healColor, heal);
        if (fill != null) fill.color = color;
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(health) + " / " + healthSource.MaxHealth;

        sparkleTimer -= dt;
        if (healingSparkles && target > shown + 0.001f && sparkleTimer <= 0f)
        {
            SpawnSparkle();
            sparkleTimer = 0.06f;
        }
        UpdateFloating(dt);
    }

    private void CreateTrail()
    {
        if (trail != null || fill == null) return;
        var go = new GameObject("Damage Trail", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        trail = go.GetComponent<Image>();
        trail.transform.SetParent(fill.transform.parent, false);
        trail.transform.SetSiblingIndex(fill.transform.GetSiblingIndex());
        trail.sprite = fill.sprite;
        trail.type = fill.type;
        trail.fillMethod = fill.fillMethod;
        trail.fillOrigin = fill.fillOrigin;
        trail.fillClockwise = fill.fillClockwise;
        trail.preserveAspect = fill.preserveAspect;
        trail.raycastTarget = false;
        trail.rectTransform.pivot = fill.rectTransform.pivot;
        trail.rectTransform.sizeDelta = fill.rectTransform.sizeDelta;
        trail.rectTransform.anchoredPosition = fill.rectTransform.anchoredPosition;
        trail.rectTransform.localScale = fill.rectTransform.localScale;
        trail.rectTransform.localRotation = fill.rectTransform.localRotation;
    }

    private void UpdateTrail(float value)
    {
        if (trail == null) return;
        trail.enabled = true;
        trail.color = trailColor;
        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.one;
        if (trail.type == Image.Type.Filled)
            trail.fillAmount = value;
        else
        {
            switch (bar.direction)
            {
                case Slider.Direction.LeftToRight: max.x = value; break;
                case Slider.Direction.RightToLeft: min.x = 1f - value; break;
                case Slider.Direction.BottomToTop: max.y = value; break;
                case Slider.Direction.TopToBottom: min.y = 1f - value; break;
            }
        }
        trail.rectTransform.anchorMin = min;
        trail.rectTransform.anchorMax = max;
    }

    private Vector2 FillEdge()
    {
        Rect area = barRect.rect;
        switch (bar.direction)
        {
            case Slider.Direction.RightToLeft: return new Vector2(Mathf.Lerp(area.xMax, area.xMin, shown), area.yMax);
            case Slider.Direction.BottomToTop: return new Vector2(area.center.x, Mathf.Lerp(area.yMin, area.yMax, shown));
            case Slider.Direction.TopToBottom: return new Vector2(area.center.x, Mathf.Lerp(area.yMax, area.yMin, shown));
            default: return new Vector2(Mathf.Lerp(area.xMin, area.xMax, shown), area.yMax);
        }
    }

    private void SpawnNumber(float damage)
    {
        if (numberFont == null) numberFont = healthText != null ? healthText.font : TMP_Settings.defaultFontAsset;
        var go = new GameObject("Damage Number", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        TextMeshProUGUI label = go.GetComponent<TextMeshProUGUI>();
        label.font = numberFont;
        label.fontSize = numberFontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.text = "-" + Mathf.CeilToInt(damage);
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Overflow;
        AddFloating(label, FillEdge() + Vector2.up * numberFontSize, new Vector2(0f, numberRise), numberLifetime, trailColor);
        label.rectTransform.sizeDelta = new Vector2(120f, numberFontSize * 2f);
    }

    private void SpawnSparkle()
    {
        var go = new GameObject("Heal Sparkle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Image sparkle = go.GetComponent<Image>();
        AddFloating(sparkle, FillEdge(), new Vector2(Random.Range(-18f, 18f), Random.Range(15f, 35f)), 0.35f, healColor);
        sparkle.rectTransform.sizeDelta = Vector2.one * 3f;
        sparkle.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
    }

    private void AddFloating(Graphic graphic, Vector2 origin, Vector2 velocity, float lifetime, Color color)
    {
        graphic.transform.SetParent(transform, false);
        graphic.raycastTarget = false;
        graphic.color = color;
        graphic.rectTransform.anchorMin = graphic.rectTransform.anchorMax = barRect.pivot;
        graphic.rectTransform.anchoredPosition = origin;
        floating.Add(new FloatingGraphic { graphic = graphic, origin = origin, velocity = velocity, lifetime = Mathf.Max(0.01f, lifetime), color = color });
    }

    private void UpdateFloating(float dt)
    {
        for (int i = floating.Count - 1; i >= 0; i--)
        {
            FloatingGraphic item = floating[i];
            item.age += dt;
            if (item.graphic == null || item.age >= item.lifetime)
            {
                if (item.graphic != null) Destroy(item.graphic.gameObject);
                floating.RemoveAt(i);
                continue;
            }
            item.graphic.rectTransform.anchoredPosition = item.origin + item.velocity * item.age;
            Color color = item.color;
            color.a *= 1f - item.age / item.lifetime;
            item.graphic.color = color;
        }
    }

    private void OnDisable()
    {
        barRect.anchoredPosition = restPosition;
        barRect.localScale = restScale;
        if (fill != null) fill.color = restColor;
        if (trail != null) trail.enabled = false;
        foreach (FloatingGraphic item in floating)
            if (item.graphic != null) Destroy(item.graphic.gameObject);
        floating.Clear();
        hitTimer = healTimer = trailTimer = sparkleTimer = 0f;
        if (healthSource != null) bar.SetValueWithoutNotify(healthSource.currentHealth);
    }

    private void OnDestroy()
    {
        if (trail != null) Destroy(trail.gameObject);
    }
}

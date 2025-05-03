using UnityEngine;

public class LoopingScaler : MonoBehaviour
{
    public Vector3 MinScale = Vector3.one * 0.5f;
    public Vector3 MaxScale = Vector3.one * 1.5f;
    public float Frequency = 1.0f;

    private Vector3 centerScale;

    void Start()
    {
        centerScale = (MinScale + MaxScale) / 2f;
    }

    void Update()
    {
        float scaleFactor = (Mathf.Sin(Time.time * Frequency * Mathf.PI * 2) + 1f) / 2f;
        transform.localScale = Vector3.Lerp(MinScale, MaxScale, scaleFactor);
    }
}

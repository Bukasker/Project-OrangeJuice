using UnityEngine;

public class LoopingRotator : MonoBehaviour
{
    public float Amplitude = 15.0f;
    public float Frequency = 1.0f;

    private float startRotationZ;

    void Start()
    {
        startRotationZ = transform.eulerAngles.z;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * Frequency * 4.0f) * Amplitude;
        transform.rotation = Quaternion.Euler(0f, 0f, startRotationZ + angle);
    }
}

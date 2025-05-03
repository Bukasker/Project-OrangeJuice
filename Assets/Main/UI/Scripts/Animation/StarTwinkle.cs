using UnityEngine;
using UnityEngine.UI;

public class StarTwinkle : MonoBehaviour
{
    [SerializeField] private Image sr;
    private float timer;
    private float twinkleSpeed;
    private float alpha;

    void Start()
    {
        alpha = Random.Range(0.3f, 1f);
        twinkleSpeed = Random.Range(2f, 5f);
        timer = Random.Range(0f, Mathf.PI * 2);
    }

    void Update()
    {
        timer += Time.deltaTime * twinkleSpeed;
        float a = (Mathf.Sin(timer) + 1f) / 2f;
        sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.3f, 1f, a));
    }
}

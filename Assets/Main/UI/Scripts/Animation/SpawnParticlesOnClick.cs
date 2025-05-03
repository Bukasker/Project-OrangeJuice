using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnParticlesOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ParticleSystem particlePrefab;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (particlePrefab != null)
        {
            ParticleSystem ps = Instantiate(particlePrefab, transform.position, Quaternion.identity);

            ps.Play();

            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }
}

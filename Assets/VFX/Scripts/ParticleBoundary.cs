using UnityEngine;

public class ParticleBoundary : MonoBehaviour
{
	public Vector2 boundaryMin;
	public Vector2 boundaryMax;

	public ParticleSystem ps;

	void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		var particles = new ParticleSystem.Particle[ps.particleCount];
		int count = ps.GetParticles(particles);

		for (int i = 0; i < count; i++)
		{
			if (particles[i].position.x < boundaryMin.x || particles[i].position.x > boundaryMax.x ||
				particles[i].position.y < boundaryMin.y || particles[i].position.y > boundaryMax.y)
			{
				particles[i].remainingLifetime = 0;
			}
		}

		ps.SetParticles(particles, count);
	}
}

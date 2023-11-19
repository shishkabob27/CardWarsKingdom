using UnityEngine;

public class VFXRotateParticleScript : MonoBehaviour
{
	public bool ShouldRotateParticle;

	public Vector3 RotationAxis;

	public Vector3 axisOfRotation;

	private void Start()
	{
		axisOfRotation = base.transform.rotation.eulerAngles.normalized;
	}

	private void LateUpdate()
	{
		if (ShouldRotateParticle)
		{
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
			int particles = GetComponent<ParticleSystem>().GetParticles(array);
			for (int i = 0; i < particles; i++)
			{
				array[i].randomSeed = 0u;
				array[i].axisOfRotation = axisOfRotation;
				array[i].rotation = RotationAxis.y;
			}
			GetComponent<ParticleSystem>().SetParticles(array, particles);
		}
	}
}

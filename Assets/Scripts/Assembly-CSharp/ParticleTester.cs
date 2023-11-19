using UnityEngine;

public class ParticleTester : MonoBehaviour
{
	private ParticleSystem[] particles;

	private Animation[] animations;

	private void Start()
	{
		particles = GetComponentsInChildren<ParticleSystem>();
		animations = GetComponentsInChildren<Animation>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ParticleSystem[] array = particles;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
			Animation[] array2 = animations;
			foreach (Animation animation in array2)
			{
				animation.Play();
			}
		}
	}
}

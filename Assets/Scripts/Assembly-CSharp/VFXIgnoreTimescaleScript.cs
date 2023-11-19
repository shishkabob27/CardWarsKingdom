using UnityEngine;

public class VFXIgnoreTimescaleScript : MonoBehaviour
{
	private double lastTime;

	private ParticleSystem particle;

	private void Awake()
	{
		particle = GetComponent<ParticleSystem>();
	}

	private void Start()
	{
		lastTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float t = Time.realtimeSinceStartup - (float)lastTime;
		particle.Simulate(t, true, false);
		lastTime = Time.realtimeSinceStartup;
	}
}

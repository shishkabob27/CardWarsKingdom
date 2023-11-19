using UnityEngine;

public class Kochava_Demo_Cam : MonoBehaviour
{
	public float spinSpeed = 20f;

	public Color particleColorHealthy;

	public Color particleColorAiling;

	public Color particleColorIdle;

	public Color particleColorDead;

	public float speedHealthy;

	public float speedAiling;

	public ParticleSystem particleVisualizer;

	public int postBurst = 20;

	private float lastQueueLength;

	private void Start()
	{
	}

	private void Update()
	{
		if (Kochava.eventQueueLength == 0)
		{
			particleVisualizer.startColor = particleColorIdle;
		}
		else if (Kochava.eventPostingTime == -1f)
		{
			particleVisualizer.startColor = particleColorDead;
		}
		else
		{
			particleVisualizer.startColor = Color.Lerp(particleColorAiling, particleColorHealthy, Mathf.Lerp(speedAiling, speedHealthy, Kochava.eventPostingTime));
		}
		if ((float)Kochava.eventQueueLength != lastQueueLength)
		{
			if ((float)Kochava.eventQueueLength < lastQueueLength)
			{
				particleVisualizer.Emit(postBurst);
				particleVisualizer.Play();
			}
			lastQueueLength = Kochava.eventQueueLength;
		}
	}

	private void FixedUpdate()
	{
		base.transform.RotateAround(Vector3.zero, Vector3.up, spinSpeed * Time.deltaTime);
	}
}

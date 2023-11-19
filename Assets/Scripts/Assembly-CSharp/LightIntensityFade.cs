using UnityEngine;

public class LightIntensityFade : MonoBehaviour
{
	public float duration = 1f;

	public float delay;

	public float finalIntensity;

	private float baseIntensity;

	private float p_lifetime;

	private float p_delay;

	private void Start()
	{
		baseIntensity = GetComponent<Light>().intensity;
	}

	private void OnEnable()
	{
		p_lifetime = 0f;
		p_delay = delay;
		if (delay > 0f)
		{
			GetComponent<Light>().enabled = false;
		}
	}

	private void Update()
	{
		if (p_delay > 0f)
		{
			p_delay -= Time.deltaTime;
			if (p_delay <= 0f)
			{
				GetComponent<Light>().enabled = true;
			}
		}
		else if (p_lifetime / duration < 1f)
		{
			GetComponent<Light>().intensity = Mathf.Lerp(baseIntensity, finalIntensity, p_lifetime / duration);
			p_lifetime += Time.deltaTime;
		}
	}
}

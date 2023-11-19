using UnityEngine;

public class FX_BlinkingEyes : MonoBehaviour
{
	public GameObject fx;

	public float blinkingTimeMin;

	public float blinkingTimeMax;

	private void Start()
	{
		fx.SetActive(false);
		InvokeRepeating("BlinkingEyes", 0.5f, Random.Range(blinkingTimeMin, blinkingTimeMax));
	}

	private void Update()
	{
	}

	private void BlinkingEyes()
	{
		if (Random.Range(0, 5) == 1)
		{
			fx.SetActive(true);
		}
		else
		{
			fx.SetActive(false);
		}
	}
}

using UnityEngine;

public class FX_SpriteColorControl : MonoBehaviour
{
	public float fadingAwaySpeed;

	private GameObject footprintObj;

	private Color footprintColor;

	private float alphaValue;

	private float delayStartTimer;

	private void Start()
	{
		footprintObj = base.gameObject;
		footprintColor = footprintObj.GetComponent<SpriteRenderer>().color;
		alphaValue = footprintColor.a;
		float timer = footprintObj.GetComponent<DestroyObjectTimer>().timer;
		if (timer >= 2f)
		{
			delayStartTimer = timer / 2f;
		}
	}

	private void Update()
	{
		delayStartTimer -= Time.deltaTime;
		if (delayStartTimer <= 0f)
		{
			FadingAway();
		}
	}

	private void FadingAway()
	{
		if (alphaValue > 0f)
		{
			alphaValue -= Time.deltaTime * fadingAwaySpeed;
			footprintColor.a = alphaValue;
			footprintObj.GetComponent<SpriteRenderer>().color = footprintColor;
		}
	}
}

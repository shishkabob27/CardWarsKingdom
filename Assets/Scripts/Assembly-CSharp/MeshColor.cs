using UnityEngine;

public class MeshColor : MonoBehaviour
{
	private Color mPulseColor;

	private float mTimer;

	private float mPulseTime = -1f;

	public bool DisableColorPulse;

	public void SetColorPulse(Color color, float time)
	{
		mPulseTime = time;
		mPulseColor = color;
	}

	public void StopColorPulse()
	{
		mPulseTime = -1f;
	}

	private void Update()
	{
		if (DisableColorPulse)
		{
			return;
		}
		if (mPulseTime != -1f)
		{
			mTimer += Time.deltaTime;
			if (mTimer >= mPulseTime)
			{
				mTimer = 0f;
			}
			float num = 2f * mTimer / mPulseTime;
			if (num > 1f)
			{
				num = 2f - num;
			}
			base.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, mPulseColor, num);
		}
		else
		{
			mTimer = 0f;
			base.gameObject.GetComponent<Renderer>().material.color = Color.white;
		}
	}
}

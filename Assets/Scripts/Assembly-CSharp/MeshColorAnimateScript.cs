using UnityEngine;

public class MeshColorAnimateScript : MonoBehaviour
{
	public bool TriggerAnim;

	public float from;

	public float to = 1f;

	public float duration = 0.5f;

	private Material mMat;

	private float mTimer;

	private void Start()
	{
		mMat = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		if (TriggerAnim)
		{
			mTimer += Time.deltaTime;
			float num = mTimer / duration;
			float value = Mathf.Lerp(from, to, num);
			if (mMat.HasProperty("_AdditiveAmount"))
			{
				mMat.SetFloat("_AdditiveAmount", value);
			}
			if (num >= 1f)
			{
				TriggerAnim = false;
				mTimer = 0f;
			}
		}
	}
}

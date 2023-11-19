using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);

	public float duration = 0.1f;

	private Vector3 mScale;

	private bool mStarted;

	private void OnDisable()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenScale component = tweenTarget.GetComponent<TweenScale>();
			if (component != null)
			{
				component.value = mScale;
				component.enabled = false;
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null)
			{
				tweenTarget = base.transform;
			}
			mScale = tweenTarget.localScale;
		}
		TweenScale.Begin(tweenTarget.gameObject, duration, (!isPressed) ? mScale : Vector3.Scale(mScale, pressed)).method = UITweener.Method.EaseInOut;
	}
}

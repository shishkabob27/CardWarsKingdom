using UnityEngine;

public class MenuStackItem : MonoBehaviour
{
	public bool SeeThrough;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public EventDelegate UnloadFunction;

	private bool mQuitting;

	private void OnApplicationQuit()
	{
		mQuitting = true;
	}

	private void OnEnable()
	{
		if (!mQuitting)
		{
			Singleton<MenuStackController>.Instance.OnStackItemEnabled(this);
		}
	}

	private void OnDisable()
	{
		if (!mQuitting)
		{
			Singleton<MenuStackController>.Instance.OnStackItemDisabled(this);
		}
	}
}

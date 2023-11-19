using UnityEngine;

[AddComponentMenu("NGUI/Tween/Spring Position")]
public class SpringPosition : MonoBehaviour
{
	public delegate void OnFinished();

	public static SpringPosition current;

	public Vector3 target = Vector3.zero;

	public float strength = 10f;

	public bool worldSpace;

	public bool ignoreTimeScale;

	public bool updateScrollView;

	public OnFinished onFinished;

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	public string callWhenFinished;

	private Transform mTrans;

	private float mThreshold;

	private UIScrollView mSv;

	private void Start()
	{
		mTrans = base.transform;
		if (updateScrollView)
		{
			mSv = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		}
	}

	private void Update()
	{
		float deltaTime = ((!ignoreTimeScale) ? Time.deltaTime : RealTime.deltaTime);
		if (worldSpace)
		{
			if (mThreshold == 0f)
			{
				mThreshold = (target - mTrans.position).sqrMagnitude * 0.001f;
			}
			mTrans.position = NGUIMath.SpringLerp(mTrans.position, target, strength, deltaTime);
			if (mThreshold >= (target - mTrans.position).sqrMagnitude)
			{
				mTrans.position = target;
				NotifyListeners();
				base.enabled = false;
			}
		}
		else
		{
			if (mThreshold == 0f)
			{
				mThreshold = (target - mTrans.localPosition).sqrMagnitude * 1E-05f;
			}
			mTrans.localPosition = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, deltaTime);
			if (mThreshold >= (target - mTrans.localPosition).sqrMagnitude)
			{
				mTrans.localPosition = target;
				NotifyListeners();
				base.enabled = false;
			}
		}
		if (mSv != null)
		{
			mSv.UpdateScrollbars(true);
		}
	}

	private void NotifyListeners()
	{
		current = this;
		if (onFinished != null)
		{
			onFinished();
		}
		if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
		{
			eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
		}
		current = null;
	}

	public static SpringPosition Begin(GameObject go, Vector3 pos, float strength)
	{
		SpringPosition springPosition = go.GetComponent<SpringPosition>();
		if (springPosition == null)
		{
			springPosition = go.AddComponent<SpringPosition>();
		}
		springPosition.target = pos;
		springPosition.strength = strength;
		springPosition.onFinished = null;
		if (!springPosition.enabled)
		{
			springPosition.mThreshold = 0f;
			springPosition.enabled = true;
		}
		return springPosition;
	}
}

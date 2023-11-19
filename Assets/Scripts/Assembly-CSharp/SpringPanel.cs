using UnityEngine;

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanel : MonoBehaviour
{
	public delegate void OnFinished();

	public static SpringPanel current;

	public Vector3 target = Vector3.zero;

	public float strength = 10f;

	public OnFinished onFinished;

	private UIPanel mPanel;

	private Transform mTrans;

	private UIScrollView mDrag;

	private void Start()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIScrollView>();
		mTrans = base.transform;
	}

	private void Update()
	{
		AdvanceTowardsPosition();
	}

	protected virtual void AdvanceTowardsPosition()
	{
		float deltaTime = RealTime.deltaTime;
		bool flag = false;
		Vector3 localPosition = mTrans.localPosition;
		Vector3 vector = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, deltaTime);
		if ((vector - target).sqrMagnitude < 0.01f)
		{
			vector = target;
			base.enabled = false;
			flag = true;
		}
		mTrans.localPosition = vector;
		Vector3 vector2 = vector - localPosition;
		Vector2 clipOffset = mPanel.clipOffset;
		clipOffset.x -= vector2.x;
		clipOffset.y -= vector2.y;
		mPanel.clipOffset = clipOffset;
		if (mDrag != null)
		{
			mDrag.UpdateScrollbars(false);
		}
		if (flag && onFinished != null)
		{
			current = this;
			onFinished();
			current = null;
		}
	}

	public static SpringPanel Begin(GameObject go, Vector3 pos, float strength)
	{
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if (springPanel == null)
		{
			springPanel = go.AddComponent<SpringPanel>();
		}
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = null;
		springPanel.enabled = true;
		return springPanel;
	}
}

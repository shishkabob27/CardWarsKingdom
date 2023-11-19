using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Scroll View")]
public class UIDragScrollView : MonoBehaviour
{
	public UIScrollView scrollView;

	[HideInInspector]
	[SerializeField]
	private UIScrollView draggablePanel;

	private Transform mTrans;

	private UIScrollView mScroll;

	private bool mAutoFind;

	private bool mStarted;

	private void OnEnable()
	{
		mTrans = base.transform;
		if (scrollView == null && draggablePanel != null)
		{
			scrollView = draggablePanel;
			draggablePanel = null;
		}
		if (mStarted && (mAutoFind || mScroll == null))
		{
			FindScrollView();
		}
	}

	private void Start()
	{
		mStarted = true;
		FindScrollView();
	}

	private void FindScrollView()
	{
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(mTrans);
		if (scrollView == null)
		{
			scrollView = uIScrollView;
			mAutoFind = true;
		}
		else if (scrollView == uIScrollView)
		{
			mAutoFind = true;
		}
		mScroll = scrollView;
	}

	private void OnPress(bool pressed)
	{
		if (mAutoFind && mScroll != scrollView)
		{
			mScroll = scrollView;
			mAutoFind = false;
		}
		if ((bool)scrollView && base.enabled && NGUITools.GetActive(base.gameObject))
		{
			scrollView.Press(pressed);
			if (!pressed && mAutoFind)
			{
				scrollView = NGUITools.FindInParents<UIScrollView>(mTrans);
				mScroll = scrollView;
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if ((bool)scrollView && NGUITools.GetActive(this))
		{
			scrollView.Drag();
		}
	}

	private void OnScroll(float delta)
	{
		if ((bool)scrollView && NGUITools.GetActive(this))
		{
			scrollView.Scroll(delta);
		}
	}
}

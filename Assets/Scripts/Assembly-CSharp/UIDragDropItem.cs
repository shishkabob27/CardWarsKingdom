using System.Collections;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	public enum Restriction
	{
		None,
		Horizontal,
		Vertical,
		PressAndHold
	}

	public Restriction restriction;

	public bool cloneOnDrag;

	[HideInInspector]
	public float pressAndHoldDelay = 1f;

	protected Transform mTrans;

	protected Transform mParent;

	protected Collider mCollider;

	protected UIButton mButton;

	protected UIRoot mRoot;

	protected UIGrid mGrid;

	protected UITable mTable;

	protected int mTouchID = int.MinValue;

	protected float mPressTime;

	protected UIDragScrollView mDragScrollView;

	protected virtual void Start()
	{
		mTrans = base.transform;
		mCollider = GetComponent<Collider>();
		mButton = GetComponent<UIButton>();
		mDragScrollView = GetComponent<UIDragScrollView>();
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			mPressTime = RealTime.time;
		}
	}

	protected virtual void OnDragStart()
	{
		if (!base.enabled || mTouchID != int.MinValue)
		{
			return;
		}
		if (restriction != 0)
		{
			if (restriction == Restriction.Horizontal)
			{
				Vector2 totalDelta = UICamera.currentTouch.totalDelta;
				if (Mathf.Abs(totalDelta.x) < Mathf.Abs(totalDelta.y))
				{
					return;
				}
			}
			else if (restriction == Restriction.Vertical)
			{
				Vector2 totalDelta2 = UICamera.currentTouch.totalDelta;
				if (Mathf.Abs(totalDelta2.x) > Mathf.Abs(totalDelta2.y))
				{
					return;
				}
			}
			else if (restriction == Restriction.PressAndHold && mPressTime + pressAndHoldDelay > RealTime.time)
			{
				return;
			}
		}
		if (cloneOnDrag)
		{
			GameObject gameObject = NGUITools.AddChild(base.transform.parent.gameObject, base.gameObject);
			gameObject.transform.localPosition = base.transform.localPosition;
			gameObject.transform.localRotation = base.transform.localRotation;
			gameObject.transform.localScale = base.transform.localScale;
			UIButtonColor component = gameObject.GetComponent<UIButtonColor>();
			if (component != null)
			{
				component.defaultColor = GetComponent<UIButtonColor>().defaultColor;
			}
			UICamera.currentTouch.dragged = gameObject;
			UIDragDropItem component2 = gameObject.GetComponent<UIDragDropItem>();
			component2.Start();
			component2.OnDragDropStart(base.gameObject);
		}
		else
		{
			OnDragDropStart();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && mTouchID == UICamera.currentTouchID)
		{
			OnDragDropMove((Vector3)delta * mRoot.pixelSizeAdjustment);
		}
	}

	private void OnDragEnd()
	{
		if (base.enabled && mTouchID == UICamera.currentTouchID)
		{
			OnDragDropRelease(UICamera.hoveredObject);
		}
	}

	protected virtual void OnDragDropStart(GameObject cloneSource = null)
	{
		if (mDragScrollView != null)
		{
			mDragScrollView.enabled = false;
		}
		if (mButton != null)
		{
			mButton.isEnabled = false;
		}
		else if (mCollider != null)
		{
			mCollider.enabled = false;
		}
		mTouchID = UICamera.currentTouchID;
		mParent = mTrans.parent;
		mRoot = NGUITools.FindInParents<UIRoot>(mParent);
		mGrid = NGUITools.FindInParents<UIGrid>(mParent);
		mTable = NGUITools.FindInParents<UITable>(mParent);
		if (UIDragDropRoot.root == null)
		{
			UIDragDropRoot uIDragDropRoot = NGUITools.FindInParents<UIDragDropRoot>(mParent);
			if (uIDragDropRoot != null)
			{
				mTrans.parent = uIDragDropRoot.transform;
			}
		}
		else if (UIDragDropRoot.root != null)
		{
			mTrans.parent = UIDragDropRoot.root;
		}
		Vector3 localPosition = mTrans.localPosition;
		localPosition.z = 0f;
		mTrans.localPosition = localPosition;
		TweenPosition component = GetComponent<TweenPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
		SpringPosition component2 = GetComponent<SpringPosition>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		NGUITools.MarkParentAsChanged(base.gameObject);
		if (mTable != null)
		{
			mTable.repositionNow = true;
		}
		if (mGrid != null)
		{
			mGrid.repositionNow = true;
		}
	}

	protected virtual void OnDragDropMove(Vector3 delta)
	{
		mTrans.localPosition += delta;
	}

	protected virtual void OnDragDropRelease(GameObject surface)
	{
		if (!cloneOnDrag)
		{
			mTouchID = int.MinValue;
			if (mButton != null)
			{
				mButton.isEnabled = true;
			}
			else if (mCollider != null)
			{
				mCollider.enabled = true;
			}
			UIDragDropContainer uIDragDropContainer = ((!surface) ? null : NGUITools.FindInParents<UIDragDropContainer>(surface));
			if (uIDragDropContainer != null)
			{
				mTrans.parent = ((!(uIDragDropContainer.reparentTarget != null)) ? uIDragDropContainer.transform : uIDragDropContainer.reparentTarget);
				Vector3 localPosition = mTrans.localPosition;
				localPosition.z = 0f;
				mTrans.localPosition = localPosition;
			}
			else
			{
				mTrans.parent = mParent;
			}
			mParent = mTrans.parent;
			mGrid = NGUITools.FindInParents<UIGrid>(mParent);
			mTable = NGUITools.FindInParents<UITable>(mParent);
			if (mDragScrollView != null)
			{
				StartCoroutine(EnableDragScrollView());
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
			if (mTable != null)
			{
				mTable.repositionNow = true;
			}
			if (mGrid != null)
			{
				mGrid.repositionNow = true;
			}
		}
		else
		{
			NGUITools.Destroy(base.gameObject);
		}
	}

	protected IEnumerator EnableDragScrollView()
	{
		yield return new WaitForEndOfFrame();
		if (mDragScrollView != null)
		{
			mDragScrollView.enabled = true;
		}
	}
}

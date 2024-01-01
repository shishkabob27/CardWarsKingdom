using System;
using UnityEngine;

public abstract class UIRect : MonoBehaviour
{
	[Serializable]
	public class AnchorPoint
	{
		public Transform target;

		public float relative;

		public int absolute;

		[NonSerialized]
		public UIRect rect;

		[NonSerialized]
		public Camera targetCam;

		public AnchorPoint()
		{
		}

		public AnchorPoint(float relative)
		{
			this.relative = relative;
		}

		public void Set(float relative, float absolute)
		{
			this.relative = relative;
			this.absolute = Mathf.FloorToInt(absolute + 0.5f);
		}

		public void Set(Transform target, float relative, float absolute)
		{
			this.target = target;
			this.relative = relative;
			this.absolute = Mathf.FloorToInt(absolute + 0.5f);
		}

		public void SetToNearest(float abs0, float abs1, float abs2)
		{
			SetToNearest(0f, 0.5f, 1f, abs0, abs1, abs2);
		}

		public void SetToNearest(float rel0, float rel1, float rel2, float abs0, float abs1, float abs2)
		{
			float num = Mathf.Abs(abs0);
			float num2 = Mathf.Abs(abs1);
			float num3 = Mathf.Abs(abs2);
			if (num < num2 && num < num3)
			{
				Set(rel0, abs0);
			}
			else if (num2 < num && num2 < num3)
			{
				Set(rel1, abs1);
			}
			else
			{
				Set(rel2, abs2);
			}
		}

		public void SetHorizontal(Transform parent, float localPos)
		{
			if ((bool)rect)
			{
				Vector3[] sides = rect.GetSides(parent);
				float num = Mathf.Lerp(sides[0].x, sides[2].x, relative);
				absolute = Mathf.FloorToInt(localPos - num + 0.5f);
				return;
			}
			Vector3 position = target.position;
			if (parent != null)
			{
				position = parent.InverseTransformPoint(position);
			}
			absolute = Mathf.FloorToInt(localPos - position.x + 0.5f);
		}

		public void SetVertical(Transform parent, float localPos)
		{
			if ((bool)rect)
			{
				Vector3[] sides = rect.GetSides(parent);
				float num = Mathf.Lerp(sides[3].y, sides[1].y, relative);
				absolute = Mathf.FloorToInt(localPos - num + 0.5f);
				return;
			}
			Vector3 position = target.position;
			if (parent != null)
			{
				position = parent.InverseTransformPoint(position);
			}
			absolute = Mathf.FloorToInt(localPos - position.y + 0.5f);
		}

		public Vector3[] GetSides(Transform relativeTo)
		{
			if (target != null)
			{
				if (rect != null)
				{
					return rect.GetSides(relativeTo);
				}
				if (target.GetComponent<Camera>() != null)
				{
					return target.GetComponent<Camera>().GetSides(relativeTo);
				}
			}
			return null;
		}
	}

	public enum AnchorUpdate
	{
		OnEnable,
		OnUpdate
	}

	public AnchorPoint leftAnchor = new AnchorPoint();

	public AnchorPoint rightAnchor = new AnchorPoint(1f);

	public AnchorPoint bottomAnchor = new AnchorPoint();

	public AnchorPoint topAnchor = new AnchorPoint(1f);

	public AnchorUpdate updateAnchors = AnchorUpdate.OnUpdate;

	protected GameObject mGo;

	protected Transform mTrans;

	protected BetterList<UIRect> mChildren = new BetterList<UIRect>();

	protected bool mChanged = true;

	protected bool mStarted;

	protected bool mParentFound;

	protected bool mUpdateAnchors;

	[NonSerialized]
	public float finalAlpha = 1f;

	private UIRoot mRoot;

	private UIRect mParent;

	private Camera mMyCam;

	private int mUpdateFrame = -1;

	private bool mAnchorsCached;

	private bool mRootSet;

	protected static Vector3[] mSides = new Vector3[4];

	public GameObject cachedGameObject
	{
		get
		{
			if (mGo == null)
			{
				mGo = base.gameObject;
			}
			return mGo;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Camera anchorCamera
	{
		get
		{
			if (!mAnchorsCached)
			{
				ResetAnchors();
			}
			return mMyCam;
		}
	}

	public bool isFullyAnchored
	{
		get
		{
			return (bool)leftAnchor.target && (bool)rightAnchor.target && (bool)topAnchor.target && (bool)bottomAnchor.target;
		}
	}

	public virtual bool isAnchoredHorizontally
	{
		get
		{
			return (bool)leftAnchor.target || (bool)rightAnchor.target;
		}
	}

	public virtual bool isAnchoredVertically
	{
		get
		{
			return (bool)bottomAnchor.target || (bool)topAnchor.target;
		}
	}

	public virtual bool canBeAnchored
	{
		get
		{
			return true;
		}
	}

	public UIRect parent
	{
		get
		{
			if (!mParentFound)
			{
				mParentFound = true;
				mParent = NGUITools.FindInParents<UIRect>(cachedTransform.parent);
			}
			return mParent;
		}
	}

	public UIRoot root
	{
		get
		{
			if (parent != null)
			{
				return mParent.root;
			}
			if (!mRootSet)
			{
				mRootSet = true;
				mRoot = NGUITools.FindInParents<UIRoot>(cachedTransform);
			}
			return mRoot;
		}
	}

	public bool isAnchored
	{
		get
		{
			return ((bool)leftAnchor.target || (bool)rightAnchor.target || (bool)topAnchor.target || (bool)bottomAnchor.target) && canBeAnchored;
		}
	}

	public abstract float alpha { get; set; }

	public abstract Vector3[] localCorners { get; }

	public abstract Vector3[] worldCorners { get; }

	public abstract float CalculateFinalAlpha(int frameID);

	public virtual void Invalidate(bool includeChildren)
	{
		mChanged = true;
		if (includeChildren)
		{
			for (int i = 0; i < mChildren.size; i++)
			{
				mChildren.buffer[i].Invalidate(true);
			}
		}
	}

	public virtual Vector3[] GetSides(Transform relativeTo)
	{
		if (anchorCamera != null)
		{
			return mMyCam.GetSides(relativeTo);
		}
		Vector3 position = cachedTransform.position;
		for (int i = 0; i < 4; i++)
		{
			mSides[i] = position;
		}
		if (relativeTo != null)
		{
			for (int j = 0; j < 4; j++)
			{
				mSides[j] = relativeTo.InverseTransformPoint(mSides[j]);
			}
		}
		return mSides;
	}

	protected Vector3 GetLocalPos(AnchorPoint ac, Transform trans)
	{
		if (anchorCamera == null || ac.targetCam == null)
		{
			return cachedTransform.localPosition;
		}
		Vector3 vector = mMyCam.ViewportToWorldPoint(ac.targetCam.WorldToViewportPoint(ac.target.position));
		if (trans != null)
		{
			vector = trans.InverseTransformPoint(vector);
		}
		vector.x = Mathf.Floor(vector.x + 0.5f);
		vector.y = Mathf.Floor(vector.y + 0.5f);
		return vector;
	}

	protected virtual void OnEnable()
	{
		mAnchorsCached = false;
		mUpdateFrame = -1;
		if (updateAnchors == AnchorUpdate.OnEnable)
		{
			mUpdateAnchors = true;
		}
		if (mStarted)
		{
			OnInit();
		}
		mUpdateFrame = -1;
	}

	protected virtual void OnInit()
	{
		mChanged = true;
		mRootSet = false;
		mParentFound = false;
		if (parent != null)
		{
			mParent.mChildren.Add(this);
		}
	}

	protected virtual void OnDisable()
	{
		if ((bool)mParent)
		{
			mParent.mChildren.Remove(this);
		}
		mParent = null;
		mRoot = null;
		mRootSet = false;
		mParentFound = false;
	}

	protected void Start()
	{
		mStarted = true;
		OnInit();
		OnStart();
	}

	public void Update()
	{
		if (!mAnchorsCached)
		{
			ResetAnchors();
		}
		int frameCount = Time.frameCount;
		if (mUpdateFrame == frameCount)
		{
			return;
		}
		if (updateAnchors == AnchorUpdate.OnUpdate || mUpdateAnchors)
		{
			mUpdateFrame = frameCount;
			mUpdateAnchors = false;
			bool flag = false;
			if ((bool)leftAnchor.target)
			{
				flag = true;
				if (leftAnchor.rect != null && leftAnchor.rect.mUpdateFrame != frameCount)
				{
					leftAnchor.rect.Update();
				}
			}
			if ((bool)bottomAnchor.target)
			{
				flag = true;
				if (bottomAnchor.rect != null && bottomAnchor.rect.mUpdateFrame != frameCount)
				{
					bottomAnchor.rect.Update();
				}
			}
			if ((bool)rightAnchor.target)
			{
				flag = true;
				if (rightAnchor.rect != null && rightAnchor.rect.mUpdateFrame != frameCount)
				{
					rightAnchor.rect.Update();
				}
			}
			if ((bool)topAnchor.target)
			{
				flag = true;
				if (topAnchor.rect != null && topAnchor.rect.mUpdateFrame != frameCount)
				{
					topAnchor.rect.Update();
				}
			}
			if (flag)
			{
				OnAnchor();
			}
		}
		OnUpdate();
	}

	public void UpdateAnchors()
	{
		if (isAnchored)
		{
			OnAnchor();
		}
	}

	protected abstract void OnAnchor();

	public void SetAnchor(Transform t)
	{
		leftAnchor.target = t;
		rightAnchor.target = t;
		topAnchor.target = t;
		bottomAnchor.target = t;
		ResetAnchors();
		UpdateAnchors();
	}

	public void SetAnchor(GameObject go)
	{
		Transform target = ((!(go != null)) ? null : go.transform);
		leftAnchor.target = target;
		rightAnchor.target = target;
		topAnchor.target = target;
		bottomAnchor.target = target;
		ResetAnchors();
		UpdateAnchors();
	}

	public void SetAnchor(GameObject go, int left, int bottom, int right, int top)
	{
		Transform target = ((!(go != null)) ? null : go.transform);
		leftAnchor.target = target;
		rightAnchor.target = target;
		topAnchor.target = target;
		bottomAnchor.target = target;
		leftAnchor.relative = 0f;
		rightAnchor.relative = 1f;
		bottomAnchor.relative = 0f;
		topAnchor.relative = 1f;
		leftAnchor.absolute = left;
		rightAnchor.absolute = right;
		bottomAnchor.absolute = bottom;
		topAnchor.absolute = top;
		ResetAnchors();
		UpdateAnchors();
	}

	public void ResetAnchors()
	{
		mAnchorsCached = true;
		leftAnchor.rect = ((!leftAnchor.target) ? null : leftAnchor.target.GetComponent<UIRect>());
		bottomAnchor.rect = ((!bottomAnchor.target) ? null : bottomAnchor.target.GetComponent<UIRect>());
		rightAnchor.rect = ((!rightAnchor.target) ? null : rightAnchor.target.GetComponent<UIRect>());
		topAnchor.rect = ((!topAnchor.target) ? null : topAnchor.target.GetComponent<UIRect>());
		mMyCam = NGUITools.FindCameraForLayer(cachedGameObject.layer);
		FindCameraFor(leftAnchor);
		FindCameraFor(bottomAnchor);
		FindCameraFor(rightAnchor);
		FindCameraFor(topAnchor);
		mUpdateAnchors = true;
	}

	public void ResetAndUpdateAnchors()
	{
		ResetAnchors();
		UpdateAnchors();
	}

	public abstract void SetRect(float x, float y, float width, float height);

	private void FindCameraFor(AnchorPoint ap)
	{
		if (ap.target == null || ap.rect != null)
		{
			ap.targetCam = null;
		}
		else
		{
			ap.targetCam = NGUITools.FindCameraForLayer(ap.target.gameObject.layer);
		}
	}

	public virtual void ParentHasChanged()
	{
		mParentFound = false;
		UIRect uIRect = NGUITools.FindInParents<UIRect>(cachedTransform.parent);
		if (mParent != uIRect)
		{
			if ((bool)mParent)
			{
				mParent.mChildren.Remove(this);
			}
			mParent = uIRect;
			if ((bool)mParent)
			{
				mParent.mChildren.Add(this);
			}
			mRootSet = false;
		}
	}

	protected abstract void OnStart();

	protected virtual void OnUpdate()
	{
	}
}

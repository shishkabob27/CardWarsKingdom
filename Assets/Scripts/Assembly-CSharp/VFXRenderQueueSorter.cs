using UnityEngine;

public class VFXRenderQueueSorter : MonoBehaviour
{
	public enum RenderSortingType
	{
		FRONT,
		BACK
	}

	public UIWidget mTarget;

	public int mQueue;

	public int mWidgetQueue;

	public RenderSortingType mType;

	public bool ShouldScaleVFX;

	private bool IsScaled;

	public bool SetQueManually;

	private Renderer[] mRenderers;

	private int mLastQueue;

	private ParticleSystem[] systems;

	private bool mReady;

	private void Awake()
	{
		mRenderers = GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		systems = GetComponentsInChildren<ParticleSystem>();
		if (ShouldScaleVFX)
		{
			ParticleSystem[] array = systems;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.enableEmission = false;
			}
		}
	}

	private void StartParticleAfterScale()
	{
		ParticleSystem[] array = systems;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.enableEmission = true;
			particleSystem.Play(true);
		}
	}

	private void Update()
	{
		if (mReady)
		{
			return;
		}
		if (base.transform.parent != null)
		{
			if (base.transform.parent.gameObject.activeInHierarchy)
			{
				mReady = true;
			}
		}
		else if (SetQueManually)
		{
			mReady = true;
		}
	}

	private void FixedUpdate()
	{
		if (!mReady)
		{
			return;
		}
		if (ShouldScaleVFX && !IsScaled)
		{
			ApplyScaleParticle();
			StartParticleAfterScale();
			IsScaled = true;
		}
		if (!SetQueManually)
		{
			if (mTarget == null)
			{
				mTarget = GetClosestParentUIWidget();
				if (!SetQueManually)
				{
					return;
				}
			}
			if (mTarget == null || mTarget.drawCall == null)
			{
				return;
			}
			mWidgetQueue = mTarget.drawCall.renderQueue;
			mQueue = ((mType != 0) ? (mWidgetQueue + -1) : (mWidgetQueue + 1));
		}
		if (mLastQueue != mQueue)
		{
			mLastQueue = mQueue;
			Renderer[] array = mRenderers;
			foreach (Renderer renderer in array)
			{
				renderer.material.renderQueue = mLastQueue;
			}
		}
	}

	private UIWidget GetClosestParentUIWidget()
	{
		UIWidget uIWidget = null;
		bool flag = false;
		Transform parent = base.transform.parent;
		while (parent != null && uIWidget == null)
		{
			UIWidget component = parent.GetComponent<UIWidget>();
			UITexture[] componentsInChildren = parent.GetComponentsInChildren<UITexture>();
			UITexture[] array = componentsInChildren;
			foreach (UITexture uITexture in array)
			{
				if (uITexture.drawCall != null)
				{
					return uITexture;
				}
			}
			UILabel[] componentsInChildren2 = parent.GetComponentsInChildren<UILabel>();
			UILabel[] array2 = componentsInChildren2;
			foreach (UILabel uILabel in array2)
			{
				if (uILabel.drawCall != null)
				{
					uIWidget = uILabel;
					return uILabel;
				}
			}
			UISprite[] componentsInChildren3 = parent.GetComponentsInChildren<UISprite>();
			UISprite[] array3 = componentsInChildren3;
			foreach (UISprite uISprite in array3)
			{
				if (uISprite.drawCall != null)
				{
					uIWidget = uISprite;
					return uISprite;
				}
			}
			parent = parent.parent;
		}
		return uIWidget;
	}

	private void ApplyScaleParticle()
	{
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.startSpeed *= Mathf.Abs(base.transform.lossyScale.x);
			particleSystem.startSize *= Mathf.Abs(base.transform.lossyScale.x);
		}
	}
}

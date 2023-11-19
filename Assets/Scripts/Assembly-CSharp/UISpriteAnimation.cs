using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation")]
[ExecuteInEditMode]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	protected int mFPS = 30;

	[HideInInspector]
	[SerializeField]
	protected string mPrefix = string.Empty;

	[SerializeField]
	[HideInInspector]
	protected bool mLoop = true;

	[HideInInspector]
	[SerializeField]
	protected bool mSnap = true;

	protected UISprite mSprite;

	protected float mDelta;

	protected int mIndex;

	protected bool mActive = true;

	protected List<string> mSpriteNames = new List<string>();

	public int frames
	{
		get
		{
			return mSpriteNames.Count;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return mFPS;
		}
		set
		{
			mFPS = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return mPrefix;
		}
		set
		{
			if (mPrefix != value)
			{
				mPrefix = value;
				RebuildSpriteList();
			}
		}
	}

	public bool loop
	{
		get
		{
			return mLoop;
		}
		set
		{
			mLoop = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return mActive;
		}
	}

	protected virtual void Start()
	{
		RebuildSpriteList();
	}

	protected virtual void Update()
	{
		if (!mActive || mSpriteNames.Count <= 1 || !Application.isPlaying || !((float)mFPS > 0f))
		{
			return;
		}
		mDelta += RealTime.deltaTime;
		float num = 1f / (float)mFPS;
		if (!(num < mDelta))
		{
			return;
		}
		mDelta = ((!(num > 0f)) ? 0f : (mDelta - num));
		if (++mIndex >= mSpriteNames.Count)
		{
			mIndex = 0;
			mActive = loop;
		}
		if (mActive)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			if (mSnap)
			{
				mSprite.MakePixelPerfect();
			}
		}
	}

	public void RebuildSpriteList()
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (!(mSprite != null) || !(mSprite.atlas != null))
		{
			return;
		}
		List<UISpriteData> spriteList = mSprite.atlas.spriteList;
		int i = 0;
		for (int count = spriteList.Count; i < count; i++)
		{
			UISpriteData uISpriteData = spriteList[i];
			if (string.IsNullOrEmpty(mPrefix) || uISpriteData.name.StartsWith(mPrefix))
			{
				mSpriteNames.Add(uISpriteData.name);
			}
		}
		mSpriteNames.Sort();
	}

	public void Reset()
	{
		mActive = true;
		mIndex = 0;
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			if (mSnap)
			{
				mSprite.MakePixelPerfect();
			}
		}
	}
}

using UnityEngine;

public class UI2DSpriteAnimation : MonoBehaviour
{
	public int framerate = 20;

	public bool ignoreTimeScale = true;

	public Sprite[] frames;

	private SpriteRenderer mUnitySprite;

	private UI2DSprite mNguiSprite;

	private int mIndex;

	private float mUpdate;

	private void Start()
	{
		mUnitySprite = GetComponent<SpriteRenderer>();
		mNguiSprite = GetComponent<UI2DSprite>();
		if (framerate > 0)
		{
			mUpdate = ((!ignoreTimeScale) ? Time.time : RealTime.time) + 1f / (float)framerate;
		}
	}

	private void Update()
	{
		if (framerate == 0 || frames == null || frames.Length <= 0)
		{
			return;
		}
		float num = ((!ignoreTimeScale) ? Time.time : RealTime.time);
		if (mUpdate < num)
		{
			mUpdate = num;
			mIndex = NGUIMath.RepeatIndex((framerate <= 0) ? (mIndex - 1) : (mIndex + 1), frames.Length);
			mUpdate = num + Mathf.Abs(1f / (float)framerate);
			if (mUnitySprite != null)
			{
				mUnitySprite.sprite = frames[mIndex];
			}
			else if (mNguiSprite != null)
			{
				mNguiSprite.nextSprite = frames[mIndex];
			}
		}
	}
}

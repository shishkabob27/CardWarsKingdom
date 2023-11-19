using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Typewriter Effect")]
[RequireComponent(typeof(UILabel))]
public class TypewriterEffect : MonoBehaviour
{
	private struct FadeEntry
	{
		public int index;

		public string text;

		public float alpha;
	}

	public static TypewriterEffect current;

	public int charsPerSecond = 20;

	public float fadeInTime;

	public float delayOnPeriod;

	public float delayOnNewLine;

	public UIScrollView scrollView;

	public bool keepFullDimensions;

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	private UILabel mLabel;

	private string mFullText = string.Empty;

	private int mCurrentOffset;

	private float mNextChar;

	private bool mReset = true;

	private bool mActive;

	private BetterList<FadeEntry> mFade = new BetterList<FadeEntry>();

	public bool isActive
	{
		get
		{
			return mActive;
		}
	}

	public void ResetToBeginning()
	{
		Finish();
		mReset = true;
		mActive = true;
	}

	public void Finish()
	{
		if (mActive)
		{
			mActive = false;
			if (!mReset)
			{
				mCurrentOffset = mFullText.Length;
				mFade.Clear();
				mLabel.text = mFullText;
			}
			if (keepFullDimensions && scrollView != null)
			{
				scrollView.UpdatePosition();
			}
			current = this;
			EventDelegate.Execute(onFinished);
			current = null;
		}
	}

	private void OnEnable()
	{
		mReset = true;
		mActive = true;
	}

	private void Update()
	{
		if (!mActive)
		{
			return;
		}
		if (mReset)
		{
			mCurrentOffset = 0;
			mReset = false;
			mLabel = GetComponent<UILabel>();
			mFullText = mLabel.processedText;
			mFade.Clear();
			if (keepFullDimensions && scrollView != null)
			{
				scrollView.UpdatePosition();
			}
		}
		while (mCurrentOffset < mFullText.Length && mNextChar <= RealTime.time)
		{
			int num = mCurrentOffset;
			charsPerSecond = Mathf.Max(1, charsPerSecond);
			while (NGUIText.ParseSymbol(mFullText, ref mCurrentOffset))
			{
			}
			mCurrentOffset++;
			float num2 = 1f / (float)charsPerSecond;
			char c = ((num >= mFullText.Length) ? '\n' : mFullText[num]);
			if (c == '\n')
			{
				num2 += delayOnNewLine;
			}
			else if (num + 1 == mFullText.Length || mFullText[num + 1] <= ' ')
			{
				switch (c)
				{
				case '.':
					if (num + 2 < mFullText.Length && mFullText[num + 1] == '.' && mFullText[num + 2] == '.')
					{
						num2 += delayOnPeriod * 3f;
						num += 2;
					}
					else
					{
						num2 += delayOnPeriod;
					}
					break;
				case '!':
				case '?':
					num2 += delayOnPeriod;
					break;
				}
			}
			if (mNextChar == 0f)
			{
				mNextChar = RealTime.time + num2;
			}
			else
			{
				mNextChar += num2;
			}
			if (fadeInTime != 0f)
			{
				FadeEntry item = default(FadeEntry);
				item.index = num;
				item.alpha = 0f;
				item.text = mFullText.Substring(num, mCurrentOffset - num);
				mFade.Add(item);
			}
			else
			{
				mLabel.text = ((!keepFullDimensions) ? mFullText.Substring(0, mCurrentOffset) : (mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset)));
				if (!keepFullDimensions && scrollView != null)
				{
					scrollView.UpdatePosition();
				}
			}
		}
		if (mFade.size != 0)
		{
			int num3 = 0;
			while (num3 < mFade.size)
			{
				FadeEntry value = mFade[num3];
				value.alpha += RealTime.deltaTime / fadeInTime;
				if (value.alpha < 1f)
				{
					mFade[num3] = value;
					num3++;
				}
				else
				{
					mFade.RemoveAt(num3);
				}
			}
			if (mFade.size == 0)
			{
				if (keepFullDimensions)
				{
					mLabel.text = mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset);
				}
				else
				{
					mLabel.text = mFullText.Substring(0, mCurrentOffset);
				}
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < mFade.size; i++)
			{
				FadeEntry fadeEntry = mFade[i];
				if (i == 0)
				{
					stringBuilder.Append(mFullText.Substring(0, fadeEntry.index));
				}
				stringBuilder.Append('[');
				stringBuilder.Append(NGUIText.EncodeAlpha(fadeEntry.alpha));
				stringBuilder.Append(']');
				stringBuilder.Append(fadeEntry.text);
			}
			if (keepFullDimensions)
			{
				stringBuilder.Append("[00]");
				stringBuilder.Append(mFullText.Substring(mCurrentOffset));
			}
			mLabel.text = stringBuilder.ToString();
		}
		else if (mCurrentOffset == mFullText.Length)
		{
			current = this;
			EventDelegate.Execute(onFinished);
			current = null;
			mActive = false;
		}
	}
}

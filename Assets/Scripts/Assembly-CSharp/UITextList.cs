using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	public enum Style
	{
		Text,
		Chat
	}

	protected class Paragraph
	{
		public string text;

		public string[] lines;
	}

	public UILabel textLabel;

	public UIProgressBar scrollBar;

	public Style style;

	public int paragraphHistory = 50;

	protected char[] mSeparator = new char[1] { '\n' };

	protected BetterList<Paragraph> mParagraphs = new BetterList<Paragraph>();

	protected float mScroll;

	protected int mTotalLines;

	protected int mLastWidth;

	protected int mLastHeight;

	public bool isValid
	{
		get
		{
			return textLabel != null && textLabel.ambigiousFont != null;
		}
	}

	public float scrollValue
	{
		get
		{
			return mScroll;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (isValid && mScroll != value)
			{
				if (scrollBar != null)
				{
					scrollBar.value = value;
					return;
				}
				mScroll = value;
				UpdateVisibleText();
			}
		}
	}

	protected float lineHeight
	{
		get
		{
			return (!(textLabel != null)) ? 20f : ((float)(textLabel.fontSize + textLabel.spacingY));
		}
	}

	protected int scrollHeight
	{
		get
		{
			if (!isValid)
			{
				return 0;
			}
			int num = Mathf.FloorToInt((float)textLabel.height / lineHeight);
			return Mathf.Max(0, mTotalLines - num);
		}
	}

	public void Clear()
	{
		mParagraphs.Clear();
		UpdateVisibleText();
	}

	private void Start()
	{
		if (textLabel == null)
		{
			textLabel = GetComponentInChildren<UILabel>();
		}
		if (scrollBar != null)
		{
			EventDelegate.Add(scrollBar.onChange, OnScrollBar);
		}
		textLabel.overflowMethod = UILabel.Overflow.ClampContent;
		if (style == Style.Chat)
		{
			textLabel.pivot = UIWidget.Pivot.BottomLeft;
			scrollValue = 1f;
		}
		else
		{
			textLabel.pivot = UIWidget.Pivot.TopLeft;
			scrollValue = 0f;
		}
	}

	private void Update()
	{
		if (isValid && (textLabel.width != mLastWidth || textLabel.height != mLastHeight))
		{
			mLastWidth = textLabel.width;
			mLastHeight = textLabel.height;
			Rebuild();
		}
	}

	public void OnScroll(float val)
	{
		int num = scrollHeight;
		if (num != 0)
		{
			val *= lineHeight;
			scrollValue = mScroll - val / (float)num;
		}
	}

	public void OnDrag(Vector2 delta)
	{
		int num = scrollHeight;
		if (num != 0)
		{
			float num2 = delta.y / lineHeight;
			scrollValue = mScroll + num2 / (float)num;
		}
	}

	private void OnScrollBar()
	{
		mScroll = UIProgressBar.current.value;
		UpdateVisibleText();
	}

	public void Add(string text)
	{
		Add(text, true);
	}

	protected void Add(string text, bool updateVisible)
	{
		Paragraph paragraph = null;
		if (mParagraphs.size < paragraphHistory)
		{
			paragraph = new Paragraph();
		}
		else
		{
			paragraph = mParagraphs[0];
			mParagraphs.RemoveAt(0);
		}
		paragraph.text = text;
		mParagraphs.Add(paragraph);
		Rebuild();
	}

	protected void Rebuild()
	{
		if (!isValid)
		{
			return;
		}
		textLabel.UpdateNGUIText();
		NGUIText.rectHeight = 1000000;
		mTotalLines = 0;
		for (int i = 0; i < mParagraphs.size; i++)
		{
			Paragraph paragraph = mParagraphs.buffer[i];
			string finalText;
			NGUIText.WrapText(paragraph.text, out finalText);
			paragraph.lines = finalText.Split('\n');
			mTotalLines += paragraph.lines.Length;
		}
		mTotalLines = 0;
		int j = 0;
		for (int size = mParagraphs.size; j < size; j++)
		{
			mTotalLines += mParagraphs.buffer[j].lines.Length;
		}
		if (scrollBar != null)
		{
			UIScrollBar uIScrollBar = scrollBar as UIScrollBar;
			if (uIScrollBar != null)
			{
				uIScrollBar.barSize = ((mTotalLines != 0) ? (1f - (float)scrollHeight / (float)mTotalLines) : 1f);
			}
		}
		UpdateVisibleText();
	}

	protected void UpdateVisibleText()
	{
		if (!isValid)
		{
			return;
		}
		if (mTotalLines == 0)
		{
			textLabel.text = string.Empty;
			return;
		}
		int num = Mathf.FloorToInt((float)textLabel.height / lineHeight);
		int num2 = Mathf.Max(0, mTotalLines - num);
		int num3 = Mathf.RoundToInt(mScroll * (float)num2);
		if (num3 < 0)
		{
			num3 = 0;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num4 = 0;
		int size = mParagraphs.size;
		while (num > 0 && num4 < size)
		{
			Paragraph paragraph = mParagraphs.buffer[num4];
			int num5 = 0;
			int num6 = paragraph.lines.Length;
			while (num > 0 && num5 < num6)
			{
				string value = paragraph.lines[num5];
				if (num3 > 0)
				{
					num3--;
				}
				else
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("\n");
					}
					stringBuilder.Append(value);
					num--;
				}
				num5++;
			}
			num4++;
		}
		textLabel.text = stringBuilder.ToString();
	}
}

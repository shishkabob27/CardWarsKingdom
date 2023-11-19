using UnityEngine;
using System.Collections.Generic;

public class UIPopupList : UIWidgetContainer
{
	public enum Position
	{
		Auto = 0,
		Above = 1,
		Below = 2,
	}

	public UIAtlas atlas;
	public UIFont bitmapFont;
	public Font trueTypeFont;
	public int fontSize;
	public FontStyle fontStyle;
	public string backgroundSprite;
	public string highlightSprite;
	public Position position;
	public NGUIText.Alignment alignment;
	public List<string> itemsLOC;
	public List<string> items;
	public Vector2 padding;
	public Color textColor;
	public Color backgroundColor;
	public Color highlightColor;
	public bool isAnimated;
	public bool isLocalized;
	public List<EventDelegate> onChange;
	[SerializeField]
	private string mSelectedItem;
	[SerializeField]
	private GameObject eventReceiver;
	[SerializeField]
	private string functionName;
	[SerializeField]
	private float textScale;
	[SerializeField]
	private UIFont font;
	[SerializeField]
	private UILabel textLabel;
}

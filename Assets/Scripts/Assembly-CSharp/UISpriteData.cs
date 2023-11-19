using System;

[Serializable]
public class UISpriteData
{
	public string name = "Sprite";

	public int x;

	public int y;

	public int width;

	public int height;

	public int borderLeft;

	public int borderRight;

	public int borderTop;

	public int borderBottom;

	public int paddingLeft;

	public int paddingRight;

	public int paddingTop;

	public int paddingBottom;

	public bool hasBorder
	{
		get
		{
			return (borderLeft | borderRight | borderTop | borderBottom) != 0;
		}
	}

	public bool hasPadding
	{
		get
		{
			return (paddingLeft | paddingRight | paddingTop | paddingBottom) != 0;
		}
	}

	public void SetRect(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public void SetPadding(int left, int bottom, int right, int top)
	{
		paddingLeft = left;
		paddingBottom = bottom;
		paddingRight = right;
		paddingTop = top;
	}

	public void SetBorder(int left, int bottom, int right, int top)
	{
		borderLeft = left;
		borderBottom = bottom;
		borderRight = right;
		borderTop = top;
	}

	public void CopyFrom(UISpriteData sd)
	{
		name = sd.name;
		x = sd.x;
		y = sd.y;
		width = sd.width;
		height = sd.height;
		borderLeft = sd.borderLeft;
		borderRight = sd.borderRight;
		borderTop = sd.borderTop;
		borderBottom = sd.borderBottom;
		paddingLeft = sd.paddingLeft;
		paddingRight = sd.paddingRight;
		paddingTop = sd.paddingTop;
		paddingBottom = sd.paddingBottom;
	}

	public void CopyBorderFrom(UISpriteData sd)
	{
		borderLeft = sd.borderLeft;
		borderRight = sd.borderRight;
		borderTop = sd.borderTop;
		borderBottom = sd.borderBottom;
	}
}

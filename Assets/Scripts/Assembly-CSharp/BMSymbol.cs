using System;
using UnityEngine;

[Serializable]
public class BMSymbol
{
	public string sequence;

	public string spriteName;

	private UISpriteData mSprite;

	private bool mIsValid;

	private int mLength;

	private int mOffsetX;

	private int mOffsetY;

	private int mWidth;

	private int mHeight;

	private int mAdvance;

	private Rect mUV;

	public int length
	{
		get
		{
			if (mLength == 0)
			{
				mLength = sequence.Length;
			}
			return mLength;
		}
	}

	public int offsetX
	{
		get
		{
			return mOffsetX;
		}
	}

	public int offsetY
	{
		get
		{
			return mOffsetY;
		}
	}

	public int width
	{
		get
		{
			return mWidth;
		}
	}

	public int height
	{
		get
		{
			return mHeight;
		}
	}

	public int advance
	{
		get
		{
			return mAdvance;
		}
	}

	public Rect uvRect
	{
		get
		{
			return mUV;
		}
	}

	public void MarkAsChanged()
	{
		mIsValid = false;
	}

	public bool Validate(UIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (!mIsValid)
		{
			if (string.IsNullOrEmpty(spriteName))
			{
				return false;
			}
			mSprite = ((!(atlas != null)) ? null : atlas.GetSprite(spriteName));
			if (mSprite != null)
			{
				Texture texture = atlas.texture;
				if (texture == null)
				{
					mSprite = null;
				}
				else
				{
					mUV = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
					mUV = NGUIMath.ConvertToTexCoords(mUV, texture.width, texture.height);
					mOffsetX = mSprite.paddingLeft;
					mOffsetY = mSprite.paddingTop;
					mWidth = mSprite.width;
					mHeight = mSprite.height;
					mAdvance = mSprite.width + (mSprite.paddingLeft + mSprite.paddingRight);
					mIsValid = true;
				}
			}
		}
		return mSprite != null;
	}
}

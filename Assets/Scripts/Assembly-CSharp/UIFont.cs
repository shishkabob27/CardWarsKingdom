using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Font")]
public class UIFont : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[SerializeField]
	[HideInInspector]
	private BMFont mFont = new BMFont();

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private UIFont mReplacement;

	[HideInInspector]
	[SerializeField]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	[SerializeField]
	[HideInInspector]
	private Font mDynamicFont;

	[SerializeField]
	[HideInInspector]
	private int mDynamicFontSize = 16;

	[SerializeField]
	[HideInInspector]
	private FontStyle mDynamicFontStyle;

	[NonSerialized]
	private UISpriteData mSprite;

	private int mPMA = -1;

	private int mPacked = -1;

	public BMFont bmFont
	{
		get
		{
			return (!(mReplacement != null)) ? mFont : mReplacement.bmFont;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.bmFont = value;
			}
			else
			{
				mFont = value;
			}
		}
	}

	public int texWidth
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texWidth : ((mFont == null) ? 1 : mFont.texWidth);
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.texWidth = value;
			}
			else if (mFont != null)
			{
				mFont.texWidth = value;
			}
		}
	}

	public int texHeight
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texHeight : ((mFont == null) ? 1 : mFont.texHeight);
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.texHeight = value;
			}
			else if (mFont != null)
			{
				mFont.texHeight = value;
			}
		}
	}

	public bool hasSymbols
	{
		get
		{
			return (mReplacement != null) ? mReplacement.hasSymbols : (mSymbols != null && mSymbols.Count != 0);
		}
	}

	public List<BMSymbol> symbols
	{
		get
		{
			return (!(mReplacement != null)) ? mSymbols : mReplacement.symbols;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return (!(mReplacement != null)) ? mAtlas : mReplacement.atlas;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.atlas = value;
			}
			else
			{
				if (!(mAtlas != value))
				{
					return;
				}
				if (value == null)
				{
					if (mAtlas != null)
					{
						mMat = mAtlas.spriteMaterial;
					}
					if (sprite != null)
					{
						mUVRect = uvRect;
					}
				}
				mPMA = -1;
				mAtlas = value;
				MarkAsChanged();
			}
		}
	}

	public Material material
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.material;
			}
			if (mAtlas != null)
			{
				return mAtlas.spriteMaterial;
			}
			if (mMat != null)
			{
				if (mDynamicFont != null && mMat != mDynamicFont.material)
				{
					mMat.mainTexture = mDynamicFont.material.mainTexture;
				}
				return mMat;
			}
			if (mDynamicFont != null)
			{
				return mDynamicFont.material;
			}
			return null;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.material = value;
			}
			else if (mMat != value)
			{
				mPMA = -1;
				mMat = value;
				MarkAsChanged();
			}
		}
	}

	[Obsolete("Use UIFont.premultipliedAlphaShader instead")]
	public bool premultipliedAlpha
	{
		get
		{
			return premultipliedAlphaShader;
		}
	}

	public bool premultipliedAlphaShader
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlphaShader;
			}
			if (mAtlas != null)
			{
				return mAtlas.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public bool packedFontShader
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.packedFontShader;
			}
			if (mAtlas != null)
			{
				return false;
			}
			if (mPacked == -1)
			{
				Material material = this.material;
				mPacked = ((material != null && material.shader != null && material.shader.name.Contains("Packed")) ? 1 : 0);
			}
			return mPacked == 1;
		}
	}

	public Texture2D texture
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.texture;
			}
			Material material = this.material;
			return (!(material != null)) ? null : (material.mainTexture as Texture2D);
		}
	}

	public Rect uvRect
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.uvRect;
			}
			return (!(mAtlas != null) || sprite == null) ? new Rect(0f, 0f, 1f, 1f) : mUVRect;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.uvRect = value;
			}
			else if (sprite == null && mUVRect != value)
			{
				mUVRect = value;
				MarkAsChanged();
			}
		}
	}

	public string spriteName
	{
		get
		{
			return (!(mReplacement != null)) ? mFont.spriteName : mReplacement.spriteName;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteName = value;
			}
			else if (mFont.spriteName != value)
			{
				mFont.spriteName = value;
				MarkAsChanged();
			}
		}
	}

	public bool isValid
	{
		get
		{
			return mDynamicFont != null || mFont.isValid;
		}
	}

	[Obsolete("Use UIFont.defaultSize instead")]
	public int size
	{
		get
		{
			return defaultSize;
		}
		set
		{
			defaultSize = value;
		}
	}

	public int defaultSize
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.defaultSize;
			}
			if (isDynamic || mFont == null)
			{
				return mDynamicFontSize;
			}
			return mFont.charSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.defaultSize = value;
			}
			else
			{
				mDynamicFontSize = value;
			}
		}
	}

	public UISpriteData sprite
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.sprite;
			}
			if (mSprite == null && mAtlas != null && !string.IsNullOrEmpty(mFont.spriteName))
			{
				mSprite = mAtlas.GetSprite(mFont.spriteName);
				if (mSprite == null)
				{
					mSprite = mAtlas.GetSprite(base.name);
				}
				if (mSprite == null)
				{
					mFont.spriteName = null;
				}
				else
				{
					UpdateUVRect();
				}
				int i = 0;
				for (int count = mSymbols.Count; i < count; i++)
				{
					symbols[i].MarkAsChanged();
				}
			}
			return mSprite;
		}
	}

	public UIFont replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIFont uIFont = value;
			if (uIFont == this)
			{
				uIFont = null;
			}
			if (mReplacement != uIFont)
			{
				if (uIFont != null && uIFont.replacement == this)
				{
					uIFont.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsChanged();
				}
				mReplacement = uIFont;
				if (uIFont != null)
				{
					mPMA = -1;
					mMat = null;
					mFont = null;
					mDynamicFont = null;
				}
				MarkAsChanged();
			}
		}
	}

	public bool isDynamic
	{
		get
		{
			return (!(mReplacement != null)) ? (mDynamicFont != null) : mReplacement.isDynamic;
		}
	}

	public Font dynamicFont
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFont : mReplacement.dynamicFont;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFont = value;
			}
			else if (mDynamicFont != value)
			{
				if (mDynamicFont != null)
				{
					material = null;
				}
				mDynamicFont = value;
				MarkAsChanged();
			}
		}
	}

	public FontStyle dynamicFontStyle
	{
		get
		{
			return (!(mReplacement != null)) ? mDynamicFontStyle : mReplacement.dynamicFontStyle;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontStyle = value;
			}
			else if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				MarkAsChanged();
			}
		}
	}

	private Texture dynamicTexture
	{
		get
		{
			if ((bool)mReplacement)
			{
				return mReplacement.dynamicTexture;
			}
			if (isDynamic)
			{
				return mDynamicFont.material.mainTexture;
			}
			return null;
		}
	}

	private void Trim()
	{
		Texture texture = mAtlas.texture;
		if (texture != null && mSprite != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(mUVRect, this.texture.width, this.texture.height, true);
			Rect rect2 = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
			int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
			int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
			int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
			mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	private bool References(UIFont font)
	{
		if (font == null)
		{
			return false;
		}
		if (font == this)
		{
			return true;
		}
		return mReplacement != null && mReplacement.References(font);
	}

	public static bool CheckIfRelated(UIFont a, UIFont b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		if (a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0])
		{
			return true;
		}
		return a == b || a.References(b) || b.References(a);
	}

	public void MarkAsChanged()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsChanged();
		}
		mSprite = null;
		UILabel[] array = NGUITools.FindActive<UILabel>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UILabel uILabel = array[i];
			if (uILabel.enabled && NGUITools.GetActive(uILabel.gameObject) && CheckIfRelated(this, uILabel.bitmapFont))
			{
				UIFont bitmapFont = uILabel.bitmapFont;
				uILabel.bitmapFont = null;
				uILabel.bitmapFont = bitmapFont;
			}
		}
		int j = 0;
		for (int count = symbols.Count; j < count; j++)
		{
			symbols[j].MarkAsChanged();
		}
	}

	public void UpdateUVRect()
	{
		if (mAtlas == null)
		{
			return;
		}
		Texture texture = mAtlas.texture;
		if (texture != null)
		{
			mUVRect = new Rect(mSprite.x - mSprite.paddingLeft, mSprite.y - mSprite.paddingTop, mSprite.width + mSprite.paddingLeft + mSprite.paddingRight, mSprite.height + mSprite.paddingTop + mSprite.paddingBottom);
			mUVRect = NGUIMath.ConvertToTexCoords(mUVRect, texture.width, texture.height);
			if (mSprite.hasPadding)
			{
				Trim();
			}
		}
	}

	private BMSymbol GetSymbol(string sequence, bool createIfMissing)
	{
		int i = 0;
		for (int count = mSymbols.Count; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			if (bMSymbol.sequence == sequence)
			{
				return bMSymbol;
			}
		}
		if (createIfMissing)
		{
			BMSymbol bMSymbol2 = new BMSymbol();
			bMSymbol2.sequence = sequence;
			mSymbols.Add(bMSymbol2);
			return bMSymbol2;
		}
		return null;
	}

	public BMSymbol MatchSymbol(string text, int offset, int textLength)
	{
		int count = mSymbols.Count;
		if (count == 0)
		{
			return null;
		}
		textLength -= offset;
		for (int i = 0; i < count; i++)
		{
			BMSymbol bMSymbol = mSymbols[i];
			int length = bMSymbol.length;
			if (length == 0 || textLength < length)
			{
				continue;
			}
			bool flag = true;
			for (int j = 0; j < length; j++)
			{
				if (text[offset + j] != bMSymbol.sequence[j])
				{
					flag = false;
					break;
				}
			}
			if (flag && bMSymbol.Validate(atlas))
			{
				return bMSymbol;
			}
		}
		return null;
	}

	public void AddSymbol(string sequence, string spriteName)
	{
		BMSymbol symbol = GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		MarkAsChanged();
	}

	public void RemoveSymbol(string sequence)
	{
		BMSymbol symbol = GetSymbol(sequence, false);
		if (symbol != null)
		{
			symbols.Remove(symbol);
		}
		MarkAsChanged();
	}

	public void RenameSymbol(string before, string after)
	{
		BMSymbol symbol = GetSymbol(before, false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		MarkAsChanged();
	}

	public bool UsesSprite(string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			if (s.Equals(spriteName))
			{
				return true;
			}
			int i = 0;
			for (int count = symbols.Count; i < count; i++)
			{
				BMSymbol bMSymbol = symbols[i];
				if (s.Equals(bMSymbol.spriteName))
				{
					return true;
				}
			}
		}
		return false;
	}
}

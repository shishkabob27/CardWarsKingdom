using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
	[Serializable]
	private class Sprite
	{
		public string name = "Unity Bug";

		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		public bool rotated;

		public float paddingLeft;

		public float paddingRight;

		public float paddingTop;

		public float paddingBottom;

		public bool hasPadding
		{
			get
			{
				return paddingLeft != 0f || paddingRight != 0f || paddingTop != 0f || paddingBottom != 0f;
			}
		}
	}

	private enum Coordinates
	{
		Pixels,
		TexCoords
	}

	[SerializeField]
	[HideInInspector]
	private Material material;

	[SerializeField]
	[HideInInspector]
	private List<UISpriteData> mSprites = new List<UISpriteData>();

	[SerializeField]
	[HideInInspector]
	private float mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mReplacement;

	[HideInInspector]
	[SerializeField]
	private Coordinates mCoordinates;

	[HideInInspector]
	[SerializeField]
	private List<Sprite> sprites = new List<Sprite>();

	private int mPMA = -1;

	private Dictionary<string, int> mSpriteIndices = new Dictionary<string, int>();

	public Material spriteMaterial
	{
		get
		{
			return (!(mReplacement != null)) ? material : mReplacement.spriteMaterial;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteMaterial = value;
				return;
			}
			if (material == null)
			{
				mPMA = 0;
				material = value;
				return;
			}
			MarkAsChanged();
			mPMA = -1;
			material = value;
			MarkAsChanged();
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material material = spriteMaterial;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public List<UISpriteData> spriteList
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.spriteList;
			}
			if (mSprites.Count == 0)
			{
				Upgrade();
			}
			return mSprites;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteList = value;
			}
			else
			{
				mSprites = value;
			}
		}
	}

	public Texture texture
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texture : ((!(material != null)) ? null : material.mainTexture);
		}
	}

	public float pixelSize
	{
		get
		{
			return (!(mReplacement != null)) ? mPixelSize : mReplacement.pixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
				return;
			}
			float num = Mathf.Clamp(value, 0.25f, 4f);
			if (mPixelSize != num)
			{
				mPixelSize = num;
				MarkAsChanged();
			}
		}
	}

	public UIAtlas replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIAtlas uIAtlas = value;
			if (uIAtlas == this)
			{
				uIAtlas = null;
			}
			if (mReplacement != uIAtlas)
			{
				if (uIAtlas != null && uIAtlas.replacement == this)
				{
					uIAtlas.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsChanged();
				}
				mReplacement = uIAtlas;
				if (uIAtlas != null)
				{
					material = null;
				}
				MarkAsChanged();
			}
		}
	}

	public UISpriteData GetSprite(string name)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetSprite(name);
		}
		if (!string.IsNullOrEmpty(name))
		{
			if (mSprites.Count == 0)
			{
				Upgrade();
			}
			if (mSprites.Count == 0)
			{
				return null;
			}
			if (mSpriteIndices.Count != mSprites.Count)
			{
				MarkSpriteListAsChanged();
			}
			int value;
			if (mSpriteIndices.TryGetValue(name, out value))
			{
				if (value > -1 && value < mSprites.Count)
				{
					return mSprites[value];
				}
				MarkSpriteListAsChanged();
				return (!mSpriteIndices.TryGetValue(name, out value)) ? null : mSprites[value];
			}
			int i = 0;
			for (int count = mSprites.Count; i < count; i++)
			{
				UISpriteData uISpriteData = mSprites[i];
				if (!string.IsNullOrEmpty(uISpriteData.name) && name == uISpriteData.name)
				{
					MarkSpriteListAsChanged();
					return uISpriteData;
				}
			}
		}
		return null;
	}

	public void MarkSpriteListAsChanged()
	{
		mSpriteIndices.Clear();
		int i = 0;
		for (int count = mSprites.Count; i < count; i++)
		{
			mSpriteIndices[mSprites[i].name] = i;
		}
	}

	public void SortAlphabetically()
	{
		mSprites.Sort((UISpriteData s1, UISpriteData s2) => s1.name.CompareTo(s2.name));
	}

	public BetterList<string> GetListOfSprites()
	{
		if (mReplacement != null)
		{
			return mReplacement.GetListOfSprites();
		}
		if (mSprites.Count == 0)
		{
			Upgrade();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		for (int count = mSprites.Count; i < count; i++)
		{
			UISpriteData uISpriteData = mSprites[i];
			if (uISpriteData != null && !string.IsNullOrEmpty(uISpriteData.name))
			{
				betterList.Add(uISpriteData.name);
			}
		}
		return betterList;
	}

	public BetterList<string> GetListOfSprites(string match)
	{
		if ((bool)mReplacement)
		{
			return mReplacement.GetListOfSprites(match);
		}
		if (string.IsNullOrEmpty(match))
		{
			return GetListOfSprites();
		}
		if (mSprites.Count == 0)
		{
			Upgrade();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		for (int count = mSprites.Count; i < count; i++)
		{
			UISpriteData uISpriteData = mSprites[i];
			if (uISpriteData != null && !string.IsNullOrEmpty(uISpriteData.name) && string.Equals(match, uISpriteData.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(uISpriteData.name);
				return betterList;
			}
		}
		string[] array = match.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		int k = 0;
		for (int count2 = mSprites.Count; k < count2; k++)
		{
			UISpriteData uISpriteData2 = mSprites[k];
			if (uISpriteData2 == null || string.IsNullOrEmpty(uISpriteData2.name))
			{
				continue;
			}
			string text = uISpriteData2.name.ToLower();
			int num = 0;
			for (int l = 0; l < array.Length; l++)
			{
				if (text.Contains(array[l]))
				{
					num++;
				}
			}
			if (num == array.Length)
			{
				betterList.Add(uISpriteData2.name);
			}
		}
		return betterList;
	}

	private bool References(UIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (atlas == this)
		{
			return true;
		}
		return mReplacement != null && mReplacement.References(atlas);
	}

	public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		return a == b || a.References(b) || b.References(a);
	}

	public void MarkAsChanged()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsChanged();
		}
		UISprite[] array = NGUITools.FindActive<UISprite>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			UISprite uISprite = array[i];
			if (CheckIfRelated(this, uISprite.atlas))
			{
				UIAtlas atlas = uISprite.atlas;
				uISprite.atlas = null;
				uISprite.atlas = atlas;
			}
		}
		UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		int j = 0;
		for (int num2 = array2.Length; j < num2; j++)
		{
			UIFont uIFont = array2[j];
			if (CheckIfRelated(this, uIFont.atlas))
			{
				UIAtlas atlas2 = uIFont.atlas;
				uIFont.atlas = null;
				uIFont.atlas = atlas2;
			}
		}
		UILabel[] array3 = NGUITools.FindActive<UILabel>();
		int k = 0;
		for (int num3 = array3.Length; k < num3; k++)
		{
			UILabel uILabel = array3[k];
			if (uILabel.bitmapFont != null && CheckIfRelated(this, uILabel.bitmapFont.atlas))
			{
				UIFont bitmapFont = uILabel.bitmapFont;
				uILabel.bitmapFont = null;
				uILabel.bitmapFont = bitmapFont;
			}
		}
	}

	private bool Upgrade()
	{
		if ((bool)mReplacement)
		{
			return mReplacement.Upgrade();
		}
		if (mSprites.Count == 0 && sprites.Count > 0 && (bool)material)
		{
			Texture mainTexture = material.mainTexture;
			int width = ((!(mainTexture != null)) ? 512 : mainTexture.width);
			int height = ((!(mainTexture != null)) ? 512 : mainTexture.height);
			for (int i = 0; i < sprites.Count; i++)
			{
				Sprite sprite = sprites[i];
				Rect outer = sprite.outer;
				Rect inner = sprite.inner;
				if (mCoordinates == Coordinates.TexCoords)
				{
					NGUIMath.ConvertToPixels(outer, width, height, true);
					NGUIMath.ConvertToPixels(inner, width, height, true);
				}
				UISpriteData uISpriteData = new UISpriteData();
				uISpriteData.name = sprite.name;
				uISpriteData.x = Mathf.RoundToInt(outer.xMin);
				uISpriteData.y = Mathf.RoundToInt(outer.yMin);
				uISpriteData.width = Mathf.RoundToInt(outer.width);
				uISpriteData.height = Mathf.RoundToInt(outer.height);
				uISpriteData.paddingLeft = Mathf.RoundToInt(sprite.paddingLeft * outer.width);
				uISpriteData.paddingRight = Mathf.RoundToInt(sprite.paddingRight * outer.width);
				uISpriteData.paddingBottom = Mathf.RoundToInt(sprite.paddingBottom * outer.height);
				uISpriteData.paddingTop = Mathf.RoundToInt(sprite.paddingTop * outer.height);
				uISpriteData.borderLeft = Mathf.RoundToInt(inner.xMin - outer.xMin);
				uISpriteData.borderRight = Mathf.RoundToInt(outer.xMax - inner.xMax);
				uISpriteData.borderBottom = Mathf.RoundToInt(outer.yMax - inner.yMax);
				uISpriteData.borderTop = Mathf.RoundToInt(inner.yMin - outer.yMin);
				mSprites.Add(uISpriteData);
			}
			sprites.Clear();
			return true;
		}
		return false;
	}
}

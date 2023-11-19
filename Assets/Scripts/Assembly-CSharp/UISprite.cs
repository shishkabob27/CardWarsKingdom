using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Sprite")]
public class UISprite : UIBasicSprite
{
	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private string mSpriteName;

	[SerializeField]
	[HideInInspector]
	private bool mFillCenter = true;

	[NonSerialized]
	protected UISpriteData mSprite;

	[NonSerialized]
	private bool mSpriteSet;

	public static int AtlasCheckCounter;

	public override Material material
	{
		get
		{
			return (!(mAtlas != null)) ? null : mAtlas.spriteMaterial;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				RemoveFromPanel();
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string text = mSpriteName;
					mSpriteName = string.Empty;
					spriteName = text;
					MarkAsChanged();
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mChanged = true;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
		}
	}

	[Obsolete("Use 'centerType' instead")]
	public bool fillCenter
	{
		get
		{
			return centerType != AdvancedType.Invisible;
		}
		set
		{
			if (value != (centerType != AdvancedType.Invisible))
			{
				centerType = (value ? AdvancedType.Sliced : AdvancedType.Invisible);
				MarkAsChanged();
			}
		}
	}

	public override Vector4 border
	{
		get
		{
			UISpriteData atlasSprite = GetAtlasSprite();
			if (atlasSprite == null)
			{
				return base.border;
			}
			return new Vector4(atlasSprite.borderLeft, atlasSprite.borderBottom, atlasSprite.borderRight, atlasSprite.borderTop);
		}
	}

	public override float pixelSize
	{
		get
		{
			return (!(mAtlas != null)) ? 1f : mAtlas.pixelSize;
		}
	}

	public override int minWidth
	{
		get
		{
			if (type == Type.Sliced || type == Type.Advanced)
			{
				Vector4 vector = border * pixelSize;
				int num = Mathf.RoundToInt(vector.x + vector.z);
				UISpriteData atlasSprite = GetAtlasSprite();
				if (atlasSprite != null)
				{
					num += atlasSprite.paddingLeft + atlasSprite.paddingRight;
				}
				return Mathf.Max(base.minWidth, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minWidth;
		}
	}

	public override int minHeight
	{
		get
		{
			if (type == Type.Sliced || type == Type.Advanced)
			{
				Vector4 vector = border * pixelSize;
				int num = Mathf.RoundToInt(vector.y + vector.w);
				UISpriteData atlasSprite = GetAtlasSprite();
				if (atlasSprite != null)
				{
					num += atlasSprite.paddingTop + atlasSprite.paddingBottom;
				}
				return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minHeight;
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 vector = base.pivotOffset;
			float num = (0f - vector.x) * (float)mWidth;
			float num2 = (0f - vector.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			if (GetAtlasSprite() != null && mType != Type.Tiled)
			{
				int paddingLeft = mSprite.paddingLeft;
				int paddingBottom = mSprite.paddingBottom;
				int num5 = mSprite.paddingRight;
				int num6 = mSprite.paddingTop;
				int num7 = mSprite.width + paddingLeft + num5;
				int num8 = mSprite.height + paddingBottom + num6;
				float num9 = 1f;
				float num10 = 1f;
				if (num7 > 0 && num8 > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if (((uint)num7 & (true ? 1u : 0u)) != 0)
					{
						num5++;
					}
					if (((uint)num8 & (true ? 1u : 0u)) != 0)
					{
						num6++;
					}
					num9 = 1f / (float)num7 * (float)mWidth;
					num10 = 1f / (float)num8 * (float)mHeight;
				}
				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					num += (float)num5 * num9;
					num3 -= (float)paddingLeft * num9;
				}
				else
				{
					num += (float)paddingLeft * num9;
					num3 -= (float)num5 * num9;
				}
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					num2 += (float)num6 * num10;
					num4 -= (float)paddingBottom * num10;
				}
				else
				{
					num2 += (float)paddingBottom * num10;
					num4 -= (float)num6 * num10;
				}
			}
			Vector4 vector2 = ((!(mAtlas != null)) ? Vector4.zero : (border * pixelSize));
			float num11 = vector2.x + vector2.z;
			float num12 = vector2.y + vector2.w;
			float x = Mathf.Lerp(num, num3 - num11, mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num12, mDrawRegion.y);
			float z = Mathf.Lerp(num + num11, num3, mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num12, num4, mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public override bool premultipliedAlpha
	{
		get
		{
			return mAtlas != null && mAtlas.premultipliedAlpha;
		}
	}

	private void CheckForOldAtlas()
	{
		if (AtlasCheckCounter == 0)
		{
		}
		if (AtlasCheckCounter < 1000 && (bool)atlas && atlas.name.ToLower() == "guiatlas")
		{
			AtlasCheckCounter++;
			string text = base.gameObject.name;
			Transform transform = base.transform.parent;
			while ((bool)transform)
			{
				text = text + "/" + transform.name;
				transform = transform.parent;
			}
		}
	}

	public UISpriteData GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UISpriteData uISpriteData = mAtlas.spriteList[0];
				if (uISpriteData == null)
				{
					return null;
				}
				SetAtlasSprite(uISpriteData);
				if (mSprite == null)
				{
					return null;
				}
				mSpriteName = mSprite.name;
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UISpriteData sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public override void MakePixelPerfect()
	{
		if (!isValid)
		{
			return;
		}
		base.MakePixelPerfect();
		if (mType == Type.Tiled)
		{
			return;
		}
		UISpriteData atlasSprite = GetAtlasSprite();
		if (atlasSprite == null)
		{
			return;
		}
		Texture texture = mainTexture;
		if (!(texture == null) && (mType == Type.Simple || mType == Type.Filled || !atlasSprite.hasBorder) && texture != null)
		{
			int num = Mathf.RoundToInt(pixelSize * (float)(atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight));
			int num2 = Mathf.RoundToInt(pixelSize * (float)(atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom));
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
	}

	protected override void OnInit()
	{
		if (!mFillCenter)
		{
			mFillCenter = true;
			centerType = AdvancedType.Invisible;
		}
		base.OnInit();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = mainTexture;
		if (texture == null)
		{
			return;
		}
		if (mSprite == null)
		{
			mSprite = atlas.GetSprite(spriteName);
		}
		if (mSprite != null)
		{
			Rect rect = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			Rect rect2 = new Rect(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop, mSprite.width - mSprite.borderLeft - mSprite.borderRight, mSprite.height - mSprite.borderBottom - mSprite.borderTop);
			rect = NGUIMath.ConvertToTexCoords(rect, texture.width, texture.height);
			rect2 = NGUIMath.ConvertToTexCoords(rect2, texture.width, texture.height);
			int size = verts.size;
			Fill(verts, uvs, cols, rect, rect2);
			if (onPostFill != null)
			{
				onPostFill(this, size, verts, uvs, cols);
			}
		}
	}
}

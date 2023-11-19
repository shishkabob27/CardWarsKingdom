using System;
using UnityEngine;

public abstract class UIBasicSprite : UIWidget
{
	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled,
		Advanced
	}

	public enum FillDirection
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360
	}

	public enum AdvancedType
	{
		Invisible,
		Sliced,
		Tiled
	}

	public enum Flip
	{
		Nothing,
		Horizontally,
		Vertically,
		Both
	}

	[SerializeField]
	[HideInInspector]
	protected Type mType;

	[HideInInspector]
	[SerializeField]
	protected FillDirection mFillDirection = FillDirection.Radial360;

	[Range(0f, 1f)]
	[SerializeField]
	[HideInInspector]
	protected float mFillAmount = 1f;

	[HideInInspector]
	[SerializeField]
	protected bool mInvert;

	[HideInInspector]
	[SerializeField]
	protected Flip mFlip;

	[NonSerialized]
	private Rect mInnerUV = default(Rect);

	[NonSerialized]
	private Rect mOuterUV = default(Rect);

	public AdvancedType centerType = AdvancedType.Sliced;

	public AdvancedType leftType = AdvancedType.Sliced;

	public AdvancedType rightType = AdvancedType.Sliced;

	public AdvancedType bottomType = AdvancedType.Sliced;

	public AdvancedType topType = AdvancedType.Sliced;

	protected static Vector2[] mTempPos = new Vector2[4];

	protected static Vector2[] mTempUVs = new Vector2[4];

	public virtual Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	public Flip flip
	{
		get
		{
			return mFlip;
		}
		set
		{
			if (mFlip != value)
			{
				mFlip = value;
				MarkAsChanged();
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mFillAmount != num)
			{
				mFillAmount = num;
				mChanged = true;
			}
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
				return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minHeight;
		}
	}

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	public bool hasBorder
	{
		get
		{
			Vector4 vector = border;
			return vector.x != 0f || vector.y != 0f || vector.z != 0f || vector.w != 0f;
		}
	}

	public virtual bool premultipliedAlpha
	{
		get
		{
			return false;
		}
	}

	public virtual float pixelSize
	{
		get
		{
			return 1f;
		}
	}

	private Vector4 drawingUVs
	{
		get
		{
			switch (mFlip)
			{
			case Flip.Horizontally:
				return new Vector4(mOuterUV.xMax, mOuterUV.yMin, mOuterUV.xMin, mOuterUV.yMax);
			case Flip.Vertically:
				return new Vector4(mOuterUV.xMin, mOuterUV.yMax, mOuterUV.xMax, mOuterUV.yMin);
			case Flip.Both:
				return new Vector4(mOuterUV.xMax, mOuterUV.yMax, mOuterUV.xMin, mOuterUV.yMin);
			default:
				return new Vector4(mOuterUV.xMin, mOuterUV.yMin, mOuterUV.xMax, mOuterUV.yMax);
			}
		}
	}

	private Color32 drawingColor
	{
		get
		{
			Color color = base.color;
			color.a = finalAlpha;
			return (!premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		}
	}

	protected void Fill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Rect outer, Rect inner)
	{
		mOuterUV = outer;
		mInnerUV = inner;
		switch (type)
		{
		case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;
		case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;
		case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;
		case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		case Type.Advanced:
			AdvancedFill(verts, uvs, cols);
			break;
		}
	}

	private void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 vector = drawingDimensions;
		Vector4 vector2 = drawingUVs;
		Color32 item = drawingColor;
		verts.Add(new Vector3(vector.x, vector.y));
		verts.Add(new Vector3(vector.x, vector.w));
		verts.Add(new Vector3(vector.z, vector.w));
		verts.Add(new Vector3(vector.z, vector.y));
		uvs.Add(new Vector2(vector2.x, vector2.y));
		uvs.Add(new Vector2(vector2.x, vector2.w));
		uvs.Add(new Vector2(vector2.z, vector2.w));
		uvs.Add(new Vector2(vector2.z, vector2.y));
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}

	private void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 vector = border * pixelSize;
		if (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Color32 item = drawingColor;
		Vector4 vector2 = drawingDimensions;
		mTempPos[0].x = vector2.x;
		mTempPos[0].y = vector2.y;
		mTempPos[3].x = vector2.z;
		mTempPos[3].y = vector2.w;
		if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
		{
			mTempPos[1].x = mTempPos[0].x + vector.z;
			mTempPos[2].x = mTempPos[3].x - vector.x;
			mTempUVs[3].x = mOuterUV.xMin;
			mTempUVs[2].x = mInnerUV.xMin;
			mTempUVs[1].x = mInnerUV.xMax;
			mTempUVs[0].x = mOuterUV.xMax;
		}
		else
		{
			mTempPos[1].x = mTempPos[0].x + vector.x;
			mTempPos[2].x = mTempPos[3].x - vector.z;
			mTempUVs[0].x = mOuterUV.xMin;
			mTempUVs[1].x = mInnerUV.xMin;
			mTempUVs[2].x = mInnerUV.xMax;
			mTempUVs[3].x = mOuterUV.xMax;
		}
		if (mFlip == Flip.Vertically || mFlip == Flip.Both)
		{
			mTempPos[1].y = mTempPos[0].y + vector.w;
			mTempPos[2].y = mTempPos[3].y - vector.y;
			mTempUVs[3].y = mOuterUV.yMin;
			mTempUVs[2].y = mInnerUV.yMin;
			mTempUVs[1].y = mInnerUV.yMax;
			mTempUVs[0].y = mOuterUV.yMax;
		}
		else
		{
			mTempPos[1].y = mTempPos[0].y + vector.y;
			mTempPos[2].y = mTempPos[3].y - vector.w;
			mTempUVs[0].y = mOuterUV.yMin;
			mTempUVs[1].y = mInnerUV.yMin;
			mTempUVs[2].y = mInnerUV.yMax;
			mTempUVs[3].y = mOuterUV.yMax;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			for (int j = 0; j < 3; j++)
			{
				if (centerType != 0 || i != 1 || j != 1)
				{
					int num2 = j + 1;
					verts.Add(new Vector3(mTempPos[i].x, mTempPos[j].y));
					verts.Add(new Vector3(mTempPos[i].x, mTempPos[num2].y));
					verts.Add(new Vector3(mTempPos[num].x, mTempPos[num2].y));
					verts.Add(new Vector3(mTempPos[num].x, mTempPos[j].y));
					uvs.Add(new Vector2(mTempUVs[i].x, mTempUVs[j].y));
					uvs.Add(new Vector2(mTempUVs[i].x, mTempUVs[num2].y));
					uvs.Add(new Vector2(mTempUVs[num].x, mTempUVs[num2].y));
					uvs.Add(new Vector2(mTempUVs[num].x, mTempUVs[j].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}

	private void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = mainTexture;
		if (texture == null)
		{
			return;
		}
		Vector2 vector = new Vector2(mInnerUV.width * (float)texture.width, mInnerUV.height * (float)texture.height);
		vector *= pixelSize;
		if (texture == null || vector.x < 2f || vector.y < 2f)
		{
			return;
		}
		Color32 item = drawingColor;
		Vector4 vector2 = drawingDimensions;
		Vector4 vector3 = default(Vector4);
		if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
		{
			vector3.x = mInnerUV.xMax;
			vector3.z = mInnerUV.xMin;
		}
		else
		{
			vector3.x = mInnerUV.xMin;
			vector3.z = mInnerUV.xMax;
		}
		if (mFlip == Flip.Vertically || mFlip == Flip.Both)
		{
			vector3.y = mInnerUV.yMax;
			vector3.w = mInnerUV.yMin;
		}
		else
		{
			vector3.y = mInnerUV.yMin;
			vector3.w = mInnerUV.yMax;
		}
		float x = vector2.x;
		float num = vector2.y;
		float x2 = vector3.x;
		float y = vector3.y;
		for (; num < vector2.w; num += vector.y)
		{
			x = vector2.x;
			float num2 = num + vector.y;
			float y2 = vector3.w;
			if (num2 > vector2.w)
			{
				y2 = Mathf.Lerp(vector3.y, vector3.w, (vector2.w - num) / vector.y);
				num2 = vector2.w;
			}
			for (; x < vector2.z; x += vector.x)
			{
				float num3 = x + vector.x;
				float x3 = vector3.z;
				if (num3 > vector2.z)
				{
					x3 = Mathf.Lerp(vector3.x, vector3.z, (vector2.z - x) / vector.x);
					num3 = vector2.z;
				}
				verts.Add(new Vector3(x, num));
				verts.Add(new Vector3(x, num2));
				verts.Add(new Vector3(num3, num2));
				verts.Add(new Vector3(num3, num));
				uvs.Add(new Vector2(x2, y));
				uvs.Add(new Vector2(x2, y2));
				uvs.Add(new Vector2(x3, y2));
				uvs.Add(new Vector2(x3, y));
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
			}
		}
	}

	private void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mFillAmount < 0.001f)
		{
			return;
		}
		Vector4 vector = drawingDimensions;
		Vector4 vector2 = drawingUVs;
		Color32 item = drawingColor;
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			if (mFillDirection == FillDirection.Horizontal)
			{
				float num = (vector2.z - vector2.x) * mFillAmount;
				if (mInvert)
				{
					vector.x = vector.z - (vector.z - vector.x) * mFillAmount;
					vector2.x = vector2.z - num;
				}
				else
				{
					vector.z = vector.x + (vector.z - vector.x) * mFillAmount;
					vector2.z = vector2.x + num;
				}
			}
			else if (mFillDirection == FillDirection.Vertical)
			{
				float num2 = (vector2.w - vector2.y) * mFillAmount;
				if (mInvert)
				{
					vector.y = vector.w - (vector.w - vector.y) * mFillAmount;
					vector2.y = vector2.w - num2;
				}
				else
				{
					vector.w = vector.y + (vector.w - vector.y) * mFillAmount;
					vector2.w = vector2.y + num2;
				}
			}
		}
		mTempPos[0] = new Vector2(vector.x, vector.y);
		mTempPos[1] = new Vector2(vector.x, vector.w);
		mTempPos[2] = new Vector2(vector.z, vector.w);
		mTempPos[3] = new Vector2(vector.z, vector.y);
		mTempUVs[0] = new Vector2(vector2.x, vector2.y);
		mTempUVs[1] = new Vector2(vector2.x, vector2.w);
		mTempUVs[2] = new Vector2(vector2.z, vector2.w);
		mTempUVs[3] = new Vector2(vector2.z, vector2.y);
		if (mFillAmount < 1f)
		{
			if (mFillDirection == FillDirection.Radial90)
			{
				if (RadialCut(mTempPos, mTempUVs, mFillAmount, mInvert, 0))
				{
					for (int i = 0; i < 4; i++)
					{
						verts.Add(mTempPos[i]);
						uvs.Add(mTempUVs[i]);
						cols.Add(item);
					}
				}
				return;
			}
			if (mFillDirection == FillDirection.Radial180)
			{
				for (int j = 0; j < 2; j++)
				{
					float t = 0f;
					float t2 = 1f;
					float t3;
					float t4;
					if (j == 0)
					{
						t3 = 0f;
						t4 = 0.5f;
					}
					else
					{
						t3 = 0.5f;
						t4 = 1f;
					}
					mTempPos[0].x = Mathf.Lerp(vector.x, vector.z, t3);
					mTempPos[1].x = mTempPos[0].x;
					mTempPos[2].x = Mathf.Lerp(vector.x, vector.z, t4);
					mTempPos[3].x = mTempPos[2].x;
					mTempPos[0].y = Mathf.Lerp(vector.y, vector.w, t);
					mTempPos[1].y = Mathf.Lerp(vector.y, vector.w, t2);
					mTempPos[2].y = mTempPos[1].y;
					mTempPos[3].y = mTempPos[0].y;
					mTempUVs[0].x = Mathf.Lerp(vector2.x, vector2.z, t3);
					mTempUVs[1].x = mTempUVs[0].x;
					mTempUVs[2].x = Mathf.Lerp(vector2.x, vector2.z, t4);
					mTempUVs[3].x = mTempUVs[2].x;
					mTempUVs[0].y = Mathf.Lerp(vector2.y, vector2.w, t);
					mTempUVs[1].y = Mathf.Lerp(vector2.y, vector2.w, t2);
					mTempUVs[2].y = mTempUVs[1].y;
					mTempUVs[3].y = mTempUVs[0].y;
					float value = (mInvert ? (mFillAmount * 2f - (float)(1 - j)) : (fillAmount * 2f - (float)j));
					if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(value), !mInvert, NGUIMath.RepeatIndex(j + 3, 4)))
					{
						for (int k = 0; k < 4; k++)
						{
							verts.Add(mTempPos[k]);
							uvs.Add(mTempUVs[k]);
							cols.Add(item);
						}
					}
				}
				return;
			}
			if (mFillDirection == FillDirection.Radial360)
			{
				for (int l = 0; l < 4; l++)
				{
					float t5;
					float t6;
					if (l < 2)
					{
						t5 = 0f;
						t6 = 0.5f;
					}
					else
					{
						t5 = 0.5f;
						t6 = 1f;
					}
					float t7;
					float t8;
					if (l == 0 || l == 3)
					{
						t7 = 0f;
						t8 = 0.5f;
					}
					else
					{
						t7 = 0.5f;
						t8 = 1f;
					}
					mTempPos[0].x = Mathf.Lerp(vector.x, vector.z, t5);
					mTempPos[1].x = mTempPos[0].x;
					mTempPos[2].x = Mathf.Lerp(vector.x, vector.z, t6);
					mTempPos[3].x = mTempPos[2].x;
					mTempPos[0].y = Mathf.Lerp(vector.y, vector.w, t7);
					mTempPos[1].y = Mathf.Lerp(vector.y, vector.w, t8);
					mTempPos[2].y = mTempPos[1].y;
					mTempPos[3].y = mTempPos[0].y;
					mTempUVs[0].x = Mathf.Lerp(vector2.x, vector2.z, t5);
					mTempUVs[1].x = mTempUVs[0].x;
					mTempUVs[2].x = Mathf.Lerp(vector2.x, vector2.z, t6);
					mTempUVs[3].x = mTempUVs[2].x;
					mTempUVs[0].y = Mathf.Lerp(vector2.y, vector2.w, t7);
					mTempUVs[1].y = Mathf.Lerp(vector2.y, vector2.w, t8);
					mTempUVs[2].y = mTempUVs[1].y;
					mTempUVs[3].y = mTempUVs[0].y;
					float value2 = ((!mInvert) ? (mFillAmount * 4f - (float)(3 - NGUIMath.RepeatIndex(l + 2, 4))) : (mFillAmount * 4f - (float)NGUIMath.RepeatIndex(l + 2, 4)));
					if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(value2), mInvert, NGUIMath.RepeatIndex(l + 2, 4)))
					{
						for (int m = 0; m < 4; m++)
						{
							verts.Add(mTempPos[m]);
							uvs.Add(mTempUVs[m]);
							cols.Add(item);
						}
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(mTempPos[n]);
			uvs.Add(mTempUVs[n]);
			cols.Add(item);
		}
	}

	private void AdvancedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = mainTexture;
		if (texture == null)
		{
			return;
		}
		Vector4 vector = border * pixelSize;
		if (vector.x == 0f && vector.y == 0f && vector.z == 0f && vector.w == 0f)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Color32 color = drawingColor;
		Vector4 vector2 = drawingDimensions;
		Vector2 vector3 = new Vector2(mInnerUV.width * (float)texture.width, mInnerUV.height * (float)texture.height);
		vector3 *= pixelSize;
		if (vector3.x < 1f)
		{
			vector3.x = 1f;
		}
		if (vector3.y < 1f)
		{
			vector3.y = 1f;
		}
		mTempPos[0].x = vector2.x;
		mTempPos[0].y = vector2.y;
		mTempPos[3].x = vector2.z;
		mTempPos[3].y = vector2.w;
		if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
		{
			mTempPos[1].x = mTempPos[0].x + vector.z;
			mTempPos[2].x = mTempPos[3].x - vector.x;
			mTempUVs[3].x = mOuterUV.xMin;
			mTempUVs[2].x = mInnerUV.xMin;
			mTempUVs[1].x = mInnerUV.xMax;
			mTempUVs[0].x = mOuterUV.xMax;
		}
		else
		{
			mTempPos[1].x = mTempPos[0].x + vector.x;
			mTempPos[2].x = mTempPos[3].x - vector.z;
			mTempUVs[0].x = mOuterUV.xMin;
			mTempUVs[1].x = mInnerUV.xMin;
			mTempUVs[2].x = mInnerUV.xMax;
			mTempUVs[3].x = mOuterUV.xMax;
		}
		if (mFlip == Flip.Vertically || mFlip == Flip.Both)
		{
			mTempPos[1].y = mTempPos[0].y + vector.w;
			mTempPos[2].y = mTempPos[3].y - vector.y;
			mTempUVs[3].y = mOuterUV.yMin;
			mTempUVs[2].y = mInnerUV.yMin;
			mTempUVs[1].y = mInnerUV.yMax;
			mTempUVs[0].y = mOuterUV.yMax;
		}
		else
		{
			mTempPos[1].y = mTempPos[0].y + vector.y;
			mTempPos[2].y = mTempPos[3].y - vector.w;
			mTempUVs[0].y = mOuterUV.yMin;
			mTempUVs[1].y = mInnerUV.yMin;
			mTempUVs[2].y = mInnerUV.yMax;
			mTempUVs[3].y = mOuterUV.yMax;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			for (int j = 0; j < 3; j++)
			{
				if (centerType == AdvancedType.Invisible && i == 1 && j == 1)
				{
					continue;
				}
				int num2 = j + 1;
				if (i == 1 && j == 1)
				{
					if (centerType == AdvancedType.Tiled)
					{
						float x = mTempPos[i].x;
						float x2 = mTempPos[num].x;
						float y = mTempPos[j].y;
						float y2 = mTempPos[num2].y;
						float x3 = mTempUVs[i].x;
						float y3 = mTempUVs[j].y;
						for (float num3 = y; num3 < y2; num3 += vector3.y)
						{
							float num4 = x;
							float num5 = mTempUVs[num2].y;
							float num6 = num3 + vector3.y;
							if (num6 > y2)
							{
								num5 = Mathf.Lerp(y3, num5, (y2 - num3) / vector3.y);
								num6 = y2;
							}
							for (; num4 < x2; num4 += vector3.x)
							{
								float num7 = num4 + vector3.x;
								float num8 = mTempUVs[num].x;
								if (num7 > x2)
								{
									num8 = Mathf.Lerp(x3, num8, (x2 - num4) / vector3.x);
									num7 = x2;
								}
								Fill(verts, uvs, cols, num4, num7, num3, num6, x3, num8, y3, num5, color);
							}
						}
					}
					else if (centerType == AdvancedType.Sliced)
					{
						Fill(verts, uvs, cols, mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color);
					}
				}
				else if (i == 1)
				{
					if ((j == 0 && bottomType == AdvancedType.Tiled) || (j == 2 && topType == AdvancedType.Tiled))
					{
						float x4 = mTempPos[i].x;
						float x5 = mTempPos[num].x;
						float y4 = mTempPos[j].y;
						float y5 = mTempPos[num2].y;
						float x6 = mTempUVs[i].x;
						float y6 = mTempUVs[j].y;
						float y7 = mTempUVs[num2].y;
						for (float num9 = x4; num9 < x5; num9 += vector3.x)
						{
							float num10 = num9 + vector3.x;
							float num11 = mTempUVs[num].x;
							if (num10 > x5)
							{
								num11 = Mathf.Lerp(x6, num11, (x5 - num9) / vector3.x);
								num10 = x5;
							}
							Fill(verts, uvs, cols, num9, num10, y4, y5, x6, num11, y6, y7, color);
						}
					}
					else if ((j == 0 && bottomType == AdvancedType.Sliced) || (j == 2 && topType == AdvancedType.Sliced))
					{
						Fill(verts, uvs, cols, mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color);
					}
				}
				else if (j == 1)
				{
					if ((i == 0 && leftType == AdvancedType.Tiled) || (i == 2 && rightType == AdvancedType.Tiled))
					{
						float x7 = mTempPos[i].x;
						float x8 = mTempPos[num].x;
						float y8 = mTempPos[j].y;
						float y9 = mTempPos[num2].y;
						float x9 = mTempUVs[i].x;
						float x10 = mTempUVs[num].x;
						float y10 = mTempUVs[j].y;
						for (float num12 = y8; num12 < y9; num12 += vector3.y)
						{
							float num13 = mTempUVs[num2].y;
							float num14 = num12 + vector3.y;
							if (num14 > y9)
							{
								num13 = Mathf.Lerp(y10, num13, (y9 - num12) / vector3.y);
								num14 = y9;
							}
							Fill(verts, uvs, cols, x7, x8, num12, num14, x9, x10, y10, num13, color);
						}
					}
					else if ((i == 0 && leftType == AdvancedType.Sliced) || (i == 2 && rightType == AdvancedType.Sliced))
					{
						Fill(verts, uvs, cols, mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color);
					}
				}
				else
				{
					Fill(verts, uvs, cols, mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color);
				}
			}
		}
	}

	private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if ((corner & 1) == 1)
		{
			invert = !invert;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (invert)
		{
			num = 1f - num;
		}
		num *= (float)Math.PI / 2f;
		float cos = Mathf.Cos(num);
		float sin = Mathf.Sin(num);
		RadialCut(xy, cos, sin, invert, corner);
		RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
	{
		int num = NGUIMath.RepeatIndex(corner + 1, 4);
		int num2 = NGUIMath.RepeatIndex(corner + 2, 4);
		int num3 = NGUIMath.RepeatIndex(corner + 3, 4);
		if ((corner & 1) == 1)
		{
			if (sin > cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num2].x = xy[num].x;
				}
			}
			else if (cos > sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num3].y = xy[num2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (!invert)
			{
				xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
			else
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			return;
		}
		if (cos > sin)
		{
			sin /= cos;
			cos = 1f;
			if (!invert)
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				xy[num2].y = xy[num].y;
			}
		}
		else if (sin > cos)
		{
			cos /= sin;
			sin = 1f;
			if (invert)
			{
				xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				xy[num3].x = xy[num2].x;
			}
		}
		else
		{
			cos = 1f;
			sin = 1f;
		}
		if (invert)
		{
			xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
		}
		else
		{
			xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
		}
	}

	private static void Fill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, float v0x, float v1x, float v0y, float v1y, float u0x, float u1x, float u0y, float u1y, Color col)
	{
		verts.Add(new Vector3(v0x, v0y));
		verts.Add(new Vector3(v0x, v1y));
		verts.Add(new Vector3(v1x, v1y));
		verts.Add(new Vector3(v1x, v0y));
		uvs.Add(new Vector2(u0x, u0y));
		uvs.Add(new Vector2(u0x, u1y));
		uvs.Add(new Vector2(u1x, u1y));
		uvs.Add(new Vector2(u1x, u0y));
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
	}
}

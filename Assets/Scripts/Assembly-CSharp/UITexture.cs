using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Texture")]
public class UITexture : UIBasicSprite
{
	private class ReferenceCounter
	{
		public int mRefCount;

		public Texture mTexture;

		public ReferenceCounter(Texture texture)
		{
			mTexture = texture;
			mRefCount = 1;
		}

		public override string ToString()
		{
			if (mTexture == null)
			{
				return mRefCount + ": (null)";
			}
			return mRefCount + ": " + mTexture.name;
		}
	}

	[HideInInspector]
	[SerializeField]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	[SerializeField]
	[HideInInspector]
	private Vector4 mBorder = Vector4.zero;

	[SerializeField]
	[HideInInspector]
	private bool mTrackReferenceCounts;

	[SerializeField]
	private string mTexturePath;

	[NonSerialized]
	private int mPMA = -1;

	public static int CheckCounter = 0;

	private static Dictionary<string, ReferenceCounter> mTextureReferenceCounts = new Dictionary<string, ReferenceCounter>();

	private bool mNeedToCheckStartingTexture = true;

	public string StreamingInTexture { get; set; }

	public override Texture mainTexture
	{
		get
		{
			if (mTexture != null)
			{
				return mTexture;
			}
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			return null;
		}
		set
		{
			if (mTexture != value)
			{
				RemoveFromPanel();
				mTexture = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mShader = null;
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public override Shader shader
	{
		get
		{
			if (mMat != null)
			{
				return mMat.shader;
			}
			if (mShader == null)
			{
				mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				RemoveFromPanel();
				mShader = value;
				mPMA = -1;
				mMat = null;
				MarkAsChanged();
			}
		}
	}

	public override bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public override Vector4 border
	{
		get
		{
			return mBorder;
		}
		set
		{
			if (mBorder != value)
			{
				mBorder = value;
				MarkAsChanged();
			}
		}
	}

	public Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
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
			if (mTexture != null && mType != Type.Tiled)
			{
				int num5 = mTexture.width;
				int num6 = mTexture.height;
				int num7 = 0;
				int num8 = 0;
				float num9 = 1f;
				float num10 = 1f;
				if (num5 > 0 && num6 > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if (((uint)num5 & (true ? 1u : 0u)) != 0)
					{
						num7++;
					}
					if (((uint)num6 & (true ? 1u : 0u)) != 0)
					{
						num8++;
					}
					num9 = 1f / (float)num5 * (float)mWidth;
					num10 = 1f / (float)num6 * (float)mHeight;
				}
				if (mFlip == Flip.Horizontally || mFlip == Flip.Both)
				{
					num += (float)num7 * num9;
				}
				else
				{
					num3 -= (float)num7 * num9;
				}
				if (mFlip == Flip.Vertically || mFlip == Flip.Both)
				{
					num2 += (float)num8 * num10;
				}
				else
				{
					num4 -= (float)num8 * num10;
				}
			}
			Vector4 vector2 = border;
			float num11 = vector2.x + vector2.z;
			float num12 = vector2.y + vector2.w;
			float x = Mathf.Lerp(num, num3 - num11, mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num12, mDrawRegion.y);
			float z = Mathf.Lerp(num + num11, num3, mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num12, num4, mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public override void MakePixelPerfect()
	{
		base.MakePixelPerfect();
		if (mType == Type.Tiled)
		{
			return;
		}
		Texture texture = mainTexture;
		if (!(texture == null) && (mType == Type.Simple || mType == Type.Filled || !base.hasBorder) && texture != null)
		{
			int num = texture.width;
			int num2 = texture.height;
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

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = mainTexture;
		if (!(texture == null))
		{
			Rect rect = new Rect(mRect.x * (float)texture.width, mRect.y * (float)texture.height, (float)texture.width * mRect.width, (float)texture.height * mRect.height);
			Rect inner = rect;
			Vector4 vector = border;
			inner.xMin += vector.x;
			inner.yMin += vector.y;
			inner.xMax -= vector.z;
			inner.yMax -= vector.w;
			float num = 1f / (float)texture.width;
			float num2 = 1f / (float)texture.height;
			rect.xMin *= num;
			rect.xMax *= num;
			rect.yMin *= num2;
			rect.yMax *= num2;
			inner.xMin *= num;
			inner.xMax *= num;
			inner.yMin *= num2;
			inner.yMax *= num2;
			int size = verts.size;
			Fill(verts, uvs, cols, rect, inner);
			if (onPostFill != null)
			{
				onPostFill(this, size, verts, uvs, cols);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		CheckStartingTexture();
	}

	private void CheckStartingTexture()
	{
		if (!mNeedToCheckStartingTexture)
		{
			return;
		}
		mNeedToCheckStartingTexture = false;
		if (mTrackReferenceCounts && mTexture != null)
		{
			ReferenceCounter value;
			if (mTextureReferenceCounts.TryGetValue(mTexture.name, out value))
			{
				value.mRefCount++;
				return;
			}
			ReferenceCounter value2 = new ReferenceCounter(mTexture);
			mTextureReferenceCounts.Add(mTexture.name, value2);
		}
	}

	public void ReplaceTexture(string newTexturePath, bool shouldUnload = true)
	{
		string directoryName = Path.GetDirectoryName(newTexturePath);
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newTexturePath);
		if (mTexture != null && mTexture.name == fileNameWithoutExtension)
		{
			return;
		}
		string text = "Resources/";
		int num = newTexturePath.IndexOf(text);
		if (num >= 0)
		{
			newTexturePath = directoryName.Replace(newTexturePath.Substring(0, num) + text, string.Empty) + "/" + fileNameWithoutExtension;
		}
		CheckStartingTexture();
		if (shouldUnload)
		{
			UnloadTexture();
		}
		if (mTrackReferenceCounts)
		{
			string resourceName = fileNameWithoutExtension;
			ReferenceCounter value;
			if (mTextureReferenceCounts.TryGetValue(resourceName, out value))
			{
				mainTexture = value.mTexture;
				value.mRefCount++;
			}
			else
			{
				Texture texture2 = (mainTexture = (Texture)Singleton<SLOTResourceManager>.Instance.LoadResource(newTexturePath));
				if (texture2 != null)
				{
					ReferenceCounter value2 = new ReferenceCounter(texture2);
					if (!mTextureReferenceCounts.ContainsKey(texture2.name))
					{
						mTextureReferenceCounts.Add(texture2.name, value2);
					}
				}
			}
		}
		else
		{
			mainTexture = (Texture)Singleton<SLOTResourceManager>.Instance.LoadResource(newTexturePath);
		}
		mTexturePath = newTexturePath;
	}

	public static void AddTextureToCache(string newTexturePath)
	{
		string resourceName = Path.GetFileName(newTexturePath);
		if (!mTextureReferenceCounts.ContainsKey(resourceName))
		{
			Texture texture = (Texture)Singleton<SLOTResourceManager>.Instance.LoadResource(newTexturePath);
			ReferenceCounter value = new ReferenceCounter(texture);
			mTextureReferenceCounts.Add(resourceName, value);
		}
	}

	public static void AddLoadedTextureToCache(Texture texture)
	{
		if (!(texture == null))
		{
			ReferenceCounter value;
			if (mTextureReferenceCounts.TryGetValue(texture.name, out value))
			{
				value.mRefCount++;
				return;
			}
			ReferenceCounter value2 = new ReferenceCounter(texture);
			mTextureReferenceCounts.Add(texture.name, value2);
		}
	}

	public static Texture GetTextureFromCache(string textureName)
	{
		ReferenceCounter value;
		if (mTextureReferenceCounts.TryGetValue(textureName, out value))
		{
			value.mRefCount++;
			return value.mTexture;
		}
		return null;
	}

	public static void UnloadTextureFromCache(string texturePath)
	{
		string resourceName = Path.GetFileName(texturePath);
		ReferenceCounter value;
		if (mTextureReferenceCounts.TryGetValue(resourceName, out value))
		{
			value.mRefCount--;
			if (value.mRefCount <= 0)
			{
				mTextureReferenceCounts.Remove(resourceName);
				Resources.UnloadAsset(value.mTexture);
			}
		}
	}

	public void UnloadTexture()
	{
		if (mTexture == null)
		{
			return;
		}
		CheckStartingTexture();
		bool flag = true;
		ReferenceCounter value;
		if (mTrackReferenceCounts && mTextureReferenceCounts.TryGetValue(mTexture.name, out value))
		{
			value.mRefCount--;
			if (value.mRefCount <= 0)
			{
				mTextureReferenceCounts.Remove(mTexture.name);
			}
			else
			{
				mainTexture = null;
				flag = false;
			}
		}
		if (flag)
		{
			Texture assetToUnload = mTexture;
			mainTexture = null;
			Resources.UnloadAsset(assetToUnload);
		}
	}

	public static void CleanupTextureReferences()
	{
		mTextureReferenceCounts.Clear();
		UITexture[] array = UnityEngine.Object.FindObjectsOfType<UITexture>();
		foreach (UITexture uITexture in array)
		{
			if (uITexture.mTrackReferenceCounts)
			{
				AddLoadedTextureToCache(uITexture.mainTexture);
			}
		}
	}

	public void EnableReferenceCounting(bool enabled)
	{
		mTrackReferenceCounts = enabled;
	}

	public bool ReferenceCountingEnabled()
	{
		return mTrackReferenceCounts;
	}

	public static void PrintRefCounts()
	{
		foreach (ReferenceCounter value in mTextureReferenceCounts.Values)
		{
		}
	}

	private void CheckForAutoLoaderScript()
	{
		if (CheckCounter == 0)
		{
		}
		if (CheckCounter < 1000 && base.transform.GetComponent<TextureAutoloader>() == null && mainTexture != null)
		{
			CheckCounter++;
			string text = base.gameObject.name;
			Transform transform = base.transform.parent;
			while ((bool)transform)
			{
				text = text + "/" + transform.name;
				transform = transform.parent;
			}
		}
	}
}

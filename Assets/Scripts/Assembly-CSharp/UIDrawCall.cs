using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Draw Call")]
public class UIDrawCall : MonoBehaviour
{
	public enum Clipping
	{
		None = 0,
		SoftClip = 3,
		ConstrainButDontClip = 4
	}

	private const int maxIndexBufferCache = 10;

	private static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();

	private static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();

	[NonSerialized]
	[HideInInspector]
	public int depthStart = int.MaxValue;

	[NonSerialized]
	[HideInInspector]
	public int depthEnd = int.MinValue;

	[NonSerialized]
	[HideInInspector]
	public UIPanel manager;

	[NonSerialized]
	[HideInInspector]
	public UIPanel panel;

	[NonSerialized]
	[HideInInspector]
	public bool alwaysOnScreen;

	[NonSerialized]
	[HideInInspector]
	public BetterList<Vector3> verts = new BetterList<Vector3>();

	[NonSerialized]
	[HideInInspector]
	public BetterList<Vector3> norms = new BetterList<Vector3>();

	[NonSerialized]
	[HideInInspector]
	public BetterList<Vector4> tans = new BetterList<Vector4>();

	[NonSerialized]
	[HideInInspector]
	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	[NonSerialized]
	[HideInInspector]
	public BetterList<Color32> cols = new BetterList<Color32>();

	private Material mMaterial;

	private Texture mTexture;

	private Shader mShader;

	private int mClipCount;

	private Transform mTrans;

	private Mesh mMesh;

	private MeshFilter mFilter;

	private MeshRenderer mRenderer;

	private Material mDynamicMat;

	private int[] mIndices;

	private bool mRebuildMat = true;

	private bool mLegacyShader;

	private int mRenderQueue = 3000;

	private int mTriangles;

	[NonSerialized]
	public bool isDirty;

	private static List<int[]> mCache = new List<int[]>(10);

	private static int[] ClipRange = new int[4]
	{
		Shader.PropertyToID("_ClipRange0"),
		Shader.PropertyToID("_ClipRange1"),
		Shader.PropertyToID("_ClipRange2"),
		Shader.PropertyToID("_ClipRange4")
	};

	private static int[] ClipArgs = new int[4]
	{
		Shader.PropertyToID("_ClipArgs0"),
		Shader.PropertyToID("_ClipArgs1"),
		Shader.PropertyToID("_ClipArgs2"),
		Shader.PropertyToID("_ClipArgs3")
	};

	[Obsolete("Use UIDrawCall.activeList")]
	public static BetterList<UIDrawCall> list
	{
		get
		{
			return mActiveList;
		}
	}

	public static BetterList<UIDrawCall> activeList
	{
		get
		{
			return mActiveList;
		}
	}

	public static BetterList<UIDrawCall> inactiveList
	{
		get
		{
			return mInactiveList;
		}
	}

	public int renderQueue
	{
		get
		{
			return mRenderQueue;
		}
		set
		{
			if (mRenderQueue != value)
			{
				mRenderQueue = value;
				if (mDynamicMat != null)
				{
					mDynamicMat.renderQueue = value;
				}
			}
		}
	}

	public int sortingOrder
	{
		get
		{
			return (mRenderer != null) ? mRenderer.sortingOrder : 0;
		}
		set
		{
			if (mRenderer != null && mRenderer.sortingOrder != value)
			{
				mRenderer.sortingOrder = value;
			}
		}
	}

	public int finalRenderQueue
	{
		get
		{
			return (!(mDynamicMat != null)) ? mRenderQueue : mDynamicMat.renderQueue;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Material baseMaterial
	{
		get
		{
			return mMaterial;
		}
		set
		{
			if (mMaterial != value)
			{
				mMaterial = value;
				mRebuildMat = true;
			}
		}
	}

	public Material dynamicMaterial
	{
		get
		{
			return mDynamicMat;
		}
	}

	public Texture mainTexture
	{
		get
		{
			return mTexture;
		}
		set
		{
			mTexture = value;
			if (mDynamicMat != null)
			{
				mDynamicMat.mainTexture = value;
			}
		}
	}

	public Shader shader
	{
		get
		{
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
				mRebuildMat = true;
			}
		}
	}

	public int triangles
	{
		get
		{
			return (mMesh != null) ? mTriangles : 0;
		}
	}

	public bool isClipped
	{
		get
		{
			return mClipCount != 0;
		}
	}

	private void CreateMaterial()
	{
		mLegacyShader = false;
		mClipCount = panel.clipCount;
		string text = ((mShader != null) ? mShader.name : ((!(mMaterial != null)) ? "Unlit/Transparent Colored" : mMaterial.shader.name));
		text = text.Replace("GUI/Text Shader", "Unlit/Text");
		if (text.Length > 2 && text[text.Length - 2] == ' ')
		{
			int num = text[text.Length - 1];
			if (num > 48 && num <= 57)
			{
				text = text.Substring(0, text.Length - 2);
			}
		}
		if (text.StartsWith("Hidden/"))
		{
			text = text.Substring(7);
		}
		text = text.Replace(" (SoftClip)", string.Empty);
		if (mClipCount != 0)
		{
			shader = Shader.Find("Hidden/" + text + " " + mClipCount);
			if (shader == null)
			{
				Shader.Find(text + " " + mClipCount);
			}
			if (shader == null && mClipCount == 1)
			{
				mLegacyShader = true;
				shader = Shader.Find(text + " (SoftClip)");
			}
		}
		else
		{
			shader = Shader.Find(text);
		}
		if (mMaterial != null)
		{
			mDynamicMat = new Material(mMaterial);
			mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
			mDynamicMat.CopyPropertiesFromMaterial(mMaterial);
			string[] shaderKeywords = mMaterial.shaderKeywords;
			for (int i = 0; i < shaderKeywords.Length; i++)
			{
				mDynamicMat.EnableKeyword(shaderKeywords[i]);
			}
			if (shader != null)
			{
				mDynamicMat.shader = shader;
			}
			else if (mClipCount == 0)
			{
			}
		}
		else
		{
			mDynamicMat = new Material(shader);
			mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
		}
	}

	private Material RebuildMaterial()
	{
		NGUITools.DestroyImmediate(mDynamicMat);
		CreateMaterial();
		mDynamicMat.renderQueue = mRenderQueue;
		if (mTexture != null)
		{
			mDynamicMat.mainTexture = mTexture;
		}
		if (mRenderer != null)
		{
			mRenderer.sharedMaterials = new Material[1] { mDynamicMat };
		}
		return mDynamicMat;
	}

	private void UpdateMaterials()
	{
		if (mRebuildMat || mDynamicMat == null || mClipCount != panel.clipCount)
		{
			RebuildMaterial();
			mRebuildMat = false;
		}
		else if (mRenderer.sharedMaterial != mDynamicMat)
		{
			mRenderer.sharedMaterials = new Material[1] { mDynamicMat };
		}
	}

	public void UpdateGeometry()
	{
		int size = verts.size;
		if (size > 0 && size == uvs.size && size == cols.size && size % 4 == 0)
		{
			if (mFilter == null)
			{
				mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (mFilter == null)
			{
				mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (verts.size < 65000)
			{
				int num = (size >> 1) * 3;
				bool flag = mIndices == null || mIndices.Length != num;
				if (mMesh == null)
				{
					mMesh = new Mesh();
					mMesh.hideFlags = HideFlags.DontSave;
					mMesh.name = ((!(mMaterial != null)) ? "Mesh" : mMaterial.name);
					mMesh.MarkDynamic();
					flag = true;
				}
				bool flag2 = uvs.buffer.Length != verts.buffer.Length || cols.buffer.Length != verts.buffer.Length || (norms.buffer != null && norms.buffer.Length != verts.buffer.Length) || (tans.buffer != null && tans.buffer.Length != verts.buffer.Length);
				if (!flag2 && panel.renderQueue != 0)
				{
					flag2 = mMesh == null || mMesh.vertexCount != verts.buffer.Length;
				}
				mTriangles = verts.size >> 1;
				if (flag2 || verts.buffer.Length > 65000)
				{
					if (flag2 || mMesh.vertexCount != verts.size)
					{
						mMesh.Clear();
						flag = true;
					}
					mMesh.vertices = verts.ToArray();
					mMesh.uv = uvs.ToArray();
					mMesh.colors32 = cols.ToArray();
					if (norms != null)
					{
						mMesh.normals = norms.ToArray();
					}
					if (tans != null)
					{
						mMesh.tangents = tans.ToArray();
					}
				}
				else
				{
					if (mMesh.vertexCount != verts.buffer.Length)
					{
						mMesh.Clear();
						flag = true;
					}
					mMesh.vertices = verts.buffer;
					mMesh.uv = uvs.buffer;
					mMesh.colors32 = cols.buffer;
					if (norms != null)
					{
						mMesh.normals = norms.buffer;
					}
					if (tans != null)
					{
						mMesh.tangents = tans.buffer;
					}
				}
				if (flag)
				{
					mIndices = GenerateCachedIndexBuffer(size, num);
					mMesh.triangles = mIndices;
				}
				if (flag2 || !alwaysOnScreen)
				{
					mMesh.RecalculateBounds();
				}
				mFilter.mesh = mMesh;
			}
			else
			{
				mTriangles = 0;
				if (mFilter.mesh != null)
				{
					mFilter.mesh.Clear();
				}
			}
			if (mRenderer == null)
			{
				mRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (mRenderer == null)
			{
				mRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			UpdateMaterials();
		}
		else if (mFilter.mesh != null)
		{
			mFilter.mesh.Clear();
		}
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		norms.Clear();
		tans.Clear();
	}

	private int[] GenerateCachedIndexBuffer(int vertexCount, int indexCount)
	{
		int i = 0;
		for (int count = mCache.Count; i < count; i++)
		{
			int[] array = mCache[i];
			if (array != null && array.Length == indexCount)
			{
				return array;
			}
		}
		int[] array2 = new int[indexCount];
		int num = 0;
		for (int j = 0; j < vertexCount; j += 4)
		{
			array2[num++] = j;
			array2[num++] = j + 1;
			array2[num++] = j + 2;
			array2[num++] = j + 2;
			array2[num++] = j + 3;
			array2[num++] = j;
		}
		if (mCache.Count > 10)
		{
			mCache.RemoveAt(0);
		}
		mCache.Add(array2);
		return array2;
	}

	private void OnWillRenderObject()
	{
		UpdateMaterials();
		if (mDynamicMat == null || mClipCount == 0)
		{
			return;
		}
		if (!mLegacyShader)
		{
			UIPanel parentPanel = panel;
			int num = 0;
			while (parentPanel != null)
			{
				if (parentPanel.hasClipping)
				{
					float angle = 0f;
					Vector4 drawCallClipRange = parentPanel.drawCallClipRange;
					if (parentPanel != panel)
					{
						Vector3 vector = parentPanel.cachedTransform.InverseTransformPoint(panel.cachedTransform.position);
						drawCallClipRange.x -= vector.x;
						drawCallClipRange.y -= vector.y;
						Vector3 eulerAngles = panel.cachedTransform.rotation.eulerAngles;
						Vector3 eulerAngles2 = parentPanel.cachedTransform.rotation.eulerAngles;
						Vector3 vector2 = eulerAngles2 - eulerAngles;
						vector2.x = NGUIMath.WrapAngle(vector2.x);
						vector2.y = NGUIMath.WrapAngle(vector2.y);
						vector2.z = NGUIMath.WrapAngle(vector2.z);
						if (Mathf.Abs(vector2.x) > 0.001f || Mathf.Abs(vector2.y) > 0.001f)
						{
						}
						angle = vector2.z;
					}
					SetClipping(num++, drawCallClipRange, parentPanel.clipSoftness, angle);
				}
				parentPanel = parentPanel.parentPanel;
			}
		}
		else
		{
			Vector2 clipSoftness = panel.clipSoftness;
			Vector4 drawCallClipRange2 = panel.drawCallClipRange;
			Vector2 mainTextureOffset = new Vector2((0f - drawCallClipRange2.x) / drawCallClipRange2.z, (0f - drawCallClipRange2.y) / drawCallClipRange2.w);
			Vector2 mainTextureScale = new Vector2(1f / drawCallClipRange2.z, 1f / drawCallClipRange2.w);
			Vector2 vector3 = new Vector2(1000f, 1000f);
			if (clipSoftness.x > 0f)
			{
				vector3.x = drawCallClipRange2.z / clipSoftness.x;
			}
			if (clipSoftness.y > 0f)
			{
				vector3.y = drawCallClipRange2.w / clipSoftness.y;
			}
			mDynamicMat.mainTextureOffset = mainTextureOffset;
			mDynamicMat.mainTextureScale = mainTextureScale;
			mDynamicMat.SetVector("_ClipSharpness", vector3);
		}
	}

	private void SetClipping(int index, Vector4 cr, Vector2 soft, float angle)
	{
		angle *= -(float)Math.PI / 180f;
		Vector2 vector = new Vector2(1000f, 1000f);
		if (soft.x > 0f)
		{
			vector.x = cr.z / soft.x;
		}
		if (soft.y > 0f)
		{
			vector.y = cr.w / soft.y;
		}
		if (index < ClipRange.Length)
		{
			mDynamicMat.SetVector(ClipRange[index], new Vector4((0f - cr.x) / cr.z, (0f - cr.y) / cr.w, 1f / cr.z, 1f / cr.w));
			mDynamicMat.SetVector(ClipArgs[index], new Vector4(vector.x, vector.y, Mathf.Sin(angle), Mathf.Cos(angle)));
		}
	}

	private void OnEnable()
	{
		mRebuildMat = true;
	}

	private void OnDisable()
	{
		depthStart = int.MaxValue;
		depthEnd = int.MinValue;
		panel = null;
		manager = null;
		mMaterial = null;
		mTexture = null;
		NGUITools.DestroyImmediate(mDynamicMat);
		mDynamicMat = null;
		if (mRenderer != null)
		{
			mRenderer.sharedMaterials = new Material[0];
		}
	}

	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(mMesh);
	}

	public static UIDrawCall Create(UIPanel panel, Material mat, Texture tex, Shader shader)
	{
		return Create(null, panel, mat, tex, shader);
	}

	private static UIDrawCall Create(string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall uIDrawCall = Create(name);
		uIDrawCall.gameObject.layer = pan.cachedGameObject.layer;
		uIDrawCall.baseMaterial = mat;
		uIDrawCall.mainTexture = tex;
		uIDrawCall.shader = shader;
		uIDrawCall.renderQueue = pan.startingRenderQueue;
		uIDrawCall.sortingOrder = pan.sortingOrder;
		uIDrawCall.manager = pan;
		return uIDrawCall;
	}

	private static UIDrawCall Create(string name)
	{
		if (mInactiveList.size > 0)
		{
			UIDrawCall uIDrawCall = mInactiveList.Pop();
			mActiveList.Add(uIDrawCall);
			if (name != null)
			{
				uIDrawCall.name = name;
			}
			NGUITools.SetActive(uIDrawCall.gameObject, true);
			return uIDrawCall;
		}
		GameObject gameObject = new GameObject(name);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		UIDrawCall uIDrawCall2 = gameObject.AddComponent<UIDrawCall>();
		mActiveList.Add(uIDrawCall2);
		return uIDrawCall2;
	}

	public static void ClearAll()
	{
		bool isPlaying = Application.isPlaying;
		int num = mActiveList.size;
		while (num > 0)
		{
			UIDrawCall uIDrawCall = mActiveList[--num];
			if ((bool)uIDrawCall)
			{
				if (isPlaying)
				{
					NGUITools.SetActive(uIDrawCall.gameObject, false);
				}
				else
				{
					NGUITools.DestroyImmediate(uIDrawCall.gameObject);
				}
			}
		}
		mActiveList.Clear();
	}

	public static void ReleaseAll()
	{
		ClearAll();
		ReleaseInactive();
	}

	public static void ReleaseInactive()
	{
		int num = mInactiveList.size;
		while (num > 0)
		{
			UIDrawCall uIDrawCall = mInactiveList[--num];
			if ((bool)uIDrawCall)
			{
				NGUITools.DestroyImmediate(uIDrawCall.gameObject);
			}
		}
		mInactiveList.Clear();
	}

	public static int Count(UIPanel panel)
	{
		int num = 0;
		for (int i = 0; i < mActiveList.size; i++)
		{
			if (mActiveList[i].manager == panel)
			{
				num++;
			}
		}
		return num;
	}

	public static void Destroy(UIDrawCall dc)
	{
		if (!dc)
		{
			return;
		}
		if (Application.isPlaying)
		{
			if (mActiveList.Remove(dc))
			{
				NGUITools.SetActive(dc.gameObject, false);
				mInactiveList.Add(dc);
			}
		}
		else
		{
			mActiveList.Remove(dc);
			NGUITools.DestroyImmediate(dc.gameObject);
		}
	}
}

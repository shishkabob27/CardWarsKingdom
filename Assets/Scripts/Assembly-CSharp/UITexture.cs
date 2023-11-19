using UnityEngine;

public class UITexture : UIBasicSprite
{
	[SerializeField]
	private Rect mRect;
	[SerializeField]
	private Texture mTexture;
	[SerializeField]
	private Material mMat;
	[SerializeField]
	private Shader mShader;
	[SerializeField]
	private Vector4 mBorder;
	[SerializeField]
	private bool mTrackReferenceCounts;
	[SerializeField]
	private string mTexturePath;
}

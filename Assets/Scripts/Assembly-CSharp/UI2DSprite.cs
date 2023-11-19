using UnityEngine;

public class UI2DSprite : UIBasicSprite
{
	[SerializeField]
	private Sprite mSprite;
	[SerializeField]
	private Material mMat;
	[SerializeField]
	private Shader mShader;
	[SerializeField]
	private Vector4 mBorder;
	public Sprite nextSprite;
}

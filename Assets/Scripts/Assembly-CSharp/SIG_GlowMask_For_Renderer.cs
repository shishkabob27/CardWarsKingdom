using UnityEngine;
using System;

public class SIG_GlowMask_For_Renderer : MonoBehaviour
{
	[Serializable]
	public class TextureInfo
	{
		[SerializeField]
		private SIG_GlowMask_For_Renderer pGlowMaskComponent;
		[SerializeField]
		private Color pGlowTint;
		[SerializeField]
		private Texture pTexture;
		[SerializeField]
		private bool pUseMainTextureTilingOffset;
		[SerializeField]
		private Vector2 pTiling;
		[SerializeField]
		private Vector2 pOffset;
		[SerializeField]
		private bool pAffectAllInstancesOfMaterial;
	}

	public TextureInfo[] glowMasks;
	public Renderer rend;
}

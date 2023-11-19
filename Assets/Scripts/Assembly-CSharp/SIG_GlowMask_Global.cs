using UnityEngine;
using System;

public class SIG_GlowMask_Global : MonoBehaviour
{
	[Serializable]
	public class TextureInfo
	{
		[SerializeField]
		private SIG_GlowMask_Global pGlowMaskComponent;
		[SerializeField]
		private Material pMaterial;
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
	}

	public TextureInfo[] glowMasks;
}

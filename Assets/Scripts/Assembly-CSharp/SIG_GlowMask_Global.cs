using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Shader-Independent Glow/GlowMask global")]
public class SIG_GlowMask_Global : MonoBehaviour
{
	[Serializable]
	public class TextureInfo
	{
		[SerializeField]
		[HideInInspector]
		private SIG_GlowMask_Global pGlowMaskComponent;

		[SerializeField]
		[HideInInspector]
		private Material pMaterial;

		[HideInInspector]
		[SerializeField]
		private Color pGlowTint = Color.white;

		[HideInInspector]
		[SerializeField]
		private Texture pTexture;

		[SerializeField]
		[HideInInspector]
		private bool pUseMainTextureTilingOffset = true;

		[SerializeField]
		[HideInInspector]
		private Vector2 pTiling = Vector2.one;

		[SerializeField]
		[HideInInspector]
		private Vector2 pOffset = Vector2.one;

		public SIG_GlowMask_Global glowMaskComponent
		{
			get
			{
				return pGlowMaskComponent;
			}
			set
			{
				pGlowMaskComponent = value;
			}
		}

		public Material material
		{
			get
			{
				return pMaterial;
			}
			set
			{
				pMaterial = value;
				glowMaskComponent.UpdateMaskInfo();
			}
		}

		public Color glowTint
		{
			get
			{
				return pGlowTint;
			}
			set
			{
				pGlowTint = value;
				if (Application.isPlaying)
				{
					glowMaskComponent.UpdateMaskInfo();
				}
			}
		}

		public Texture texture
		{
			get
			{
				return pTexture;
			}
			set
			{
				pTexture = value;
				if (Application.isPlaying)
				{
					glowMaskComponent.UpdateMaskInfo();
				}
			}
		}

		public bool useMainTextureTilingOffset
		{
			get
			{
				return pUseMainTextureTilingOffset;
			}
			set
			{
				pUseMainTextureTilingOffset = value;
				if (Application.isPlaying)
				{
					glowMaskComponent.UpdateMaskInfo();
				}
			}
		}

		public Vector2 tiling
		{
			get
			{
				return pTiling;
			}
			set
			{
				pTiling = value;
				if (Application.isPlaying)
				{
					glowMaskComponent.UpdateMaskInfo();
				}
			}
		}

		public Vector2 offset
		{
			get
			{
				return pOffset;
			}
			set
			{
				pOffset = value;
				if (Application.isPlaying)
				{
					glowMaskComponent.UpdateMaskInfo();
				}
			}
		}
	}

	public TextureInfo[] glowMasks;

	protected void OnDisable()
	{
		for (int i = 0; i < glowMasks.Length; i++)
		{
			glowMasks[i].material.SetColor("_SIG_color", Color.black);
			glowMasks[i].material.SetTexture("_SIG_GlowMask", null);
		}
	}

	public void OnLevelWasLoaded(int level)
	{
		UpdateMaskInfo();
	}

	public void UpdateMaskInfo()
	{
		for (int i = 0; i < glowMasks.Length; i++)
		{
			if (glowMasks[i].material != null)
			{
				glowMasks[i].material.SetColor("_SIG_color", glowMasks[i].glowTint);
				glowMasks[i].material.SetTexture("_SIG_GlowMask", glowMasks[i].texture);
				if (glowMasks[i].useMainTextureTilingOffset)
				{
					glowMasks[i].material.SetTextureScale("_SIG_GlowMask", glowMasks[i].material.mainTextureScale);
					glowMasks[i].material.SetTextureOffset("_SIG_GlowMask", glowMasks[i].material.mainTextureOffset);
				}
				else
				{
					glowMasks[i].material.SetTextureScale("_SIG_GlowMask", glowMasks[i].tiling);
					glowMasks[i].material.SetTextureOffset("_SIG_GlowMask", glowMasks[i].offset);
				}
			}
		}
	}

	protected void Awake()
	{
		if (glowMasks == null)
		{
			glowMasks = new TextureInfo[0];
		}
		for (int i = 0; i < glowMasks.Length; i++)
		{
			if (glowMasks[i] == null)
			{
				glowMasks[i] = new TextureInfo();
			}
			glowMasks[i].glowMaskComponent = this;
		}
		UpdateMaskInfo();
	}
}

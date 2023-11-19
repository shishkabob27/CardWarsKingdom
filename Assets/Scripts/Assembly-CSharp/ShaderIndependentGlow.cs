using UnityEngine;

[AddComponentMenu("Image Effects/Shader-Independent Glow/Shader-Independent Glow image effect")]
public class ShaderIndependentGlow : MonoBehaviour
{
	public enum DebugModes
	{
		normal,
		show_glow_mask,
		show_blurred_mask
	}

	public LayerMask glowLayers = int.MaxValue;

	public Color globalGlowTint = Color.white;

	public float mainPower = 0.3f;

	public float blurPower = 2f;

	public int blurDownsample = 1;

	public float blurSize = 5f;

	public int blurIterations = 1;

	public int maskDownsample;

	public DebugModes debugMode;

	public bool checkRenderTypes = true;

	private Shader glowShader;

	private Shader glowShader_ortho;

	private Shader glowShaderRT;

	private Shader glowShaderRT_ortho;

	private Material curBlurMaterial;

	private Material curComposeMaterial;

	private RenderTexture glowRT;

	private RenderTexture blurRT;

	private Camera glowCamera;

	private Camera mainCam;

	private Texture2D globalTex;

	private Material blurMaterial
	{
		get
		{
			if (curBlurMaterial == null)
			{
				curBlurMaterial = new Material(Shader.Find("Hidden/SIG_FastBlur"));
				curBlurMaterial.hideFlags = HideFlags.DontSave;
			}
			return curBlurMaterial;
		}
	}

	private Material composeMaterial
	{
		get
		{
			if (curComposeMaterial == null)
			{
				curComposeMaterial = new Material(Shader.Find("Hidden/SIG_Compose"));
				curComposeMaterial.hideFlags = HideFlags.DontSave;
			}
			return curComposeMaterial;
		}
	}

	private void Awake()
	{
		if (KFFLODManager.IsLowEndDevice())
		{
			base.enabled = false;
		}
	}

	private void Blur(RenderTexture source, RenderTexture destination)
	{
		float num = 1f / (1f * (float)(1 << blurDownsample));
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num, (0f - blurSize) * num, 0f, 0f));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width >> blurDownsample;
		int height = source.height >> blurDownsample;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, blurMaterial, 0);
		for (int i = 0; i < blurIterations; i++)
		{
			float num2 = (float)i * 1f;
			blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num + num2, (0f - blurSize) * num - num2, 0f, 0f));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, blurMaterial, 1);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, blurMaterial, 2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(renderTexture, destination);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		glowCamera.CopyFrom(mainCam);
		glowCamera.rect = new Rect(0f, 0f, 1f, 1f);
		glowCamera.cullingMask = glowLayers;
		glowCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		glowCamera.clearFlags = CameraClearFlags.Color;
		glowCamera.depthTextureMode = DepthTextureMode.None;
		glowCamera.renderingPath = RenderingPath.VertexLit;
		glowRT = RenderTexture.GetTemporary(src.width >> maskDownsample, src.height >> maskDownsample, 24, RenderTextureFormat.ARGB32);
		glowCamera.targetTexture = glowRT;
		if (maskDownsample == 0)
		{
			Shader.SetGlobalFloat("_SIG_ZShift", 0.999f);
		}
		else
		{
			Shader.SetGlobalFloat("_SIG_ZShift", 0.99f);
		}
		if (glowCamera.orthographic)
		{
			if (!checkRenderTypes)
			{
				glowCamera.RenderWithShader(glowShader_ortho, string.Empty);
			}
			else
			{
				glowCamera.RenderWithShader(glowShaderRT_ortho, "RenderType");
			}
		}
		else if (!checkRenderTypes)
		{
			glowCamera.RenderWithShader(glowShader, string.Empty);
		}
		else
		{
			glowCamera.RenderWithShader(glowShaderRT, "RenderType");
		}
		blurRT = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
		Blur(glowRT, blurRT);
		composeMaterial.SetTexture("_GlowMask", glowRT);
		composeMaterial.SetTexture("_BlurMask", blurRT);
		composeMaterial.SetFloat("_GlowPower", mainPower);
		composeMaterial.SetFloat("_BlurPower", blurPower);
		composeMaterial.SetColor("_GlobalTint", globalGlowTint);
		switch (debugMode)
		{
		case DebugModes.normal:
			Graphics.Blit(src, dest, composeMaterial);
			break;
		case DebugModes.show_glow_mask:
			Graphics.Blit(glowRT, dest);
			break;
		case DebugModes.show_blurred_mask:
			Graphics.Blit(blurRT, dest);
			break;
		}
		RenderTexture.ReleaseTemporary(glowRT);
		RenderTexture.ReleaseTemporary(blurRT);
	}

	private void Start()
	{
		glowShader = Shader.Find("Hidden/ShaderIndependentGlow");
		glowShader_ortho = Shader.Find("Hidden/ShaderIndependentGlow_ortho");
		glowShaderRT = Shader.Find("Hidden/ShaderIndependentGlow_RT");
		glowShaderRT_ortho = Shader.Find("Hidden/ShaderIndependentGlow_RT_ortho");
		mainCam = GetComponent<Camera>();
		GameObject gameObject = new GameObject("ShaderIndependentGlowCamera");
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = base.transform.position;
		gameObject.transform.rotation = base.transform.rotation;
		gameObject.SetActive(false);
		glowCamera = gameObject.AddComponent<Camera>();
		glowCamera.CopyFrom(mainCam);
		glowCamera.targetTexture = glowRT;
		glowCamera.enabled = false;
		Shader.SetGlobalFloat("_SIG_ZShift", 0.01f);
	}

	private void OnEnable()
	{
		mainCam = GetComponent<Camera>();
		mainCam.depthTextureMode |= DepthTextureMode.Depth;
	}

	private void OnDisable()
	{
		if (curBlurMaterial != null)
		{
			Object.DestroyImmediate(curBlurMaterial);
		}
		if (curComposeMaterial != null)
		{
			Object.DestroyImmediate(curComposeMaterial);
		}
	}
}

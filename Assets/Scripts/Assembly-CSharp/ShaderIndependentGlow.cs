using UnityEngine;

public class ShaderIndependentGlow : MonoBehaviour
{
	public enum DebugModes
	{
		normal = 0,
		show_glow_mask = 1,
		show_blurred_mask = 2,
	}

	public LayerMask glowLayers;
	public Color globalGlowTint;
	public float mainPower;
	public float blurPower;
	public int blurDownsample;
	public float blurSize;
	public int blurIterations;
	public int maskDownsample;
	public DebugModes debugMode;
	public bool checkRenderTypes;
}

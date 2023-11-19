using UnityEngine;

public class TextureCleanup : MonoBehaviour
{
	private void Awake()
	{
		UITexture.CleanupTextureReferences();
		Resources.UnloadUnusedAssets();
	}
}

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UITexture))]
public class TextureAutoloader : MonoBehaviour
{
	public string StoredTexturePath;

	private void OnEnable()
	{
		UITexture component = GetComponent<UITexture>();
		if (!(component == null) && StoredTexturePath != null)
		{
			if (Application.isPlaying)
			{
				component.ReplaceTexture(StoredTexturePath);
			}
			else
			{
				component.mainTexture = Resources.Load(StoredTexturePath) as Texture;
			}
		}
	}

	private void OnDisable()
	{
		UITexture component = GetComponent<UITexture>();
		if (!(component == null) && StoredTexturePath != null)
		{
			if (Application.isPlaying)
			{
				component.UnloadTexture();
			}
			else
			{
				component.mainTexture = null;
			}
		}
	}

	public void SetTexturePath(string path)
	{
		if (StoredTexturePath != path)
		{
			StoredTexturePath = path;
			if (!base.gameObject.activeInHierarchy)
			{
				OnDisable();
			}
		}
	}
}

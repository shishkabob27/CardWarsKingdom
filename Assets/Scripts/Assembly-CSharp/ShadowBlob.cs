using System.Collections;
using UnityEngine;

public class ShadowBlob : MonoBehaviour
{
	private const float MaxAlpha = 0.6f;

	private const float FadeInTimeStep = 0.04f;

	private const float FadeOutTimeStep = 0.03f;

	private const float FadeAmount = 0.05f;

	private const string ResoucePath = "VFX/Textures/";

	private GameObject creature;

	private MeshRenderer ShadowMesh;

	private Texture ShadowTexture;

	private Renderer ShadowRenderer;

	private Material ShadowMat;

	public void PrepareCache(CreatureItem aCreatureItem)
	{
		ShadowRenderer = base.gameObject.GetComponent<Renderer>();
		ShadowMat = ShadowRenderer.material;
		if (!(ShadowTexture != null))
		{
			string path = "VFX/Textures/" + aCreatureItem.Form.ShadowTexture;
			ShadowTexture = (Texture)Singleton<SLOTResourceManager>.Instance.LoadResource(path);
		}
	}

	public void Spawn(GameObject aCreature, CreatureItem aCreatureItem)
	{
		if (!(aCreatureItem.Form.ShadowTexture == string.Empty) && aCreatureItem.Form.ShadowSize != 0)
		{
			creature = aCreature;
			if ((bool)ShadowRenderer)
			{
				ShadowRenderer.enabled = true;
				ShadowMat.SetColor("_TintColor", new Color(1f, 0f, 0f, 0f));
				ShadowMat.mainTexture = ShadowTexture;
				base.transform.localScale = new Vector3(aCreatureItem.Form.ShadowSize, aCreatureItem.Form.ShadowSize, 1f);
				StartCoroutine("FadeIn");
			}
		}
	}

	public void Despawn()
	{
		if (!(ShadowRenderer == null))
		{
			StartCoroutine("FadeOut");
		}
	}

	private void Update()
	{
		if ((bool)creature)
		{
			base.gameObject.transform.position = new Vector3(creature.transform.position.x, base.gameObject.transform.position.y, creature.transform.position.z);
		}
	}

	public IEnumerator FadeIn()
	{
		yield return new WaitForSeconds(1f);
		float Alpha = 0f;
		while (true)
		{
			Alpha += 0.05f;
			if (Alpha >= 0.6f)
			{
				break;
			}
			ShadowMat.SetColor("_TintColor", new Color(1f, 1f, 1f, Alpha));
			yield return new WaitForSeconds(0.04f);
		}
		ShadowMat.SetColor("_TintColor", new Color(1f, 1f, 1f, 0.6f));
	}

	public IEnumerator FadeOut()
	{
		float Alpha = 0.6f;
		while (true)
		{
			Alpha -= 0.05f;
			if (Alpha <= 0f)
			{
				break;
			}
			ShadowMat.SetColor("_TintColor", new Color(1f, 1f, 1f, Alpha));
			yield return new WaitForSeconds(0.03f);
		}
		ShadowMat.SetColor("_TintColor", new Color(1f, 1f, 1f, 0f));
		ShadowRenderer.enabled = false;
	}
}

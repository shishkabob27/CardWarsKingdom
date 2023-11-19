using System.Collections.Generic;
using UnityEngine;

public class EvoMaterialDetailsController : Singleton<EvoMaterialDetailsController>
{
	public UITweenController ShowTween;

	public UILabel Name;

	public UITexture Image;

	public UIGrid StarGrid;

	private List<GameObject> Stars = new List<GameObject>();

	private void Awake()
	{
		int num = 0;
		while (true)
		{
			Transform transform = StarGrid.transform.Find("Icon_RarityStar_" + (num + 1));
			if (transform != null)
			{
				Stars.Add(transform.gameObject);
				num++;
				continue;
			}
			break;
		}
	}

	public void Show(EvoMaterialData material)
	{
		ShowTween.Play();
		Name.text = material.Name;
		Image.ReplaceTexture(material.UITexture);
		for (int i = 0; i < Stars.Count; i++)
		{
			Stars[i].SetActive(i < material.Rarity);
		}
		StarGrid.Reposition();
	}
}

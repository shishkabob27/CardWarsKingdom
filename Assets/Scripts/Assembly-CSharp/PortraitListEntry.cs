using UnityEngine;

public class PortraitListEntry : UIStreamingGridListItem
{
	public UITexture Texture;

	public GameObject SelectedHighlight;

	public PlayerPortraitData Data { get; private set; }

	public override void Populate(object dataObj)
	{
		Data = dataObj as PlayerPortraitData;
		Data.ApplyTexture(Texture);
		if (SelectedHighlight != null)
		{
			SelectedHighlight.SetActive(Data == Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait);
		}
	}

	public override void Unload()
	{
		if (Texture.ReferenceCountingEnabled())
		{
			Texture.UnloadTexture();
		}
		else
		{
			Texture.mainTexture = null;
		}
		Texture.EnableReferenceCounting(true);
	}

	public void OnClick()
	{
		Singleton<PlayerCustomizationController>.Instance.OnClickPortraitEntry(this);
	}
}

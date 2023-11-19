using UnityEngine;

public class CardBackListEntry : UIStreamingGridListItem
{
	public UITexture Texture;

	public GameObject SelectedHighlight;

	public GameObject Shadow;

	public GameObject LockedObj;

	public CardBackData Data { get; private set; }

	public override void Populate(object dataObj)
	{
		Data = dataObj as CardBackData;
		if (dataObj == null)
		{
			if (LockedObj != null)
			{
				LockedObj.SetActive(false);
			}
			if (Shadow != null)
			{
				Shadow.SetActive(false);
			}
			Texture.ReplaceTexture("UI/CardFrames/UI_ActionCard_Frame_Back_E");
			if (SelectedHighlight != null)
			{
				SelectedHighlight.SetActive(false);
			}
		}
		else
		{
			if (LockedObj != null)
			{
				LockedObj.SetActive(false);
			}
			if (Shadow != null)
			{
				Shadow.SetActive(true);
			}
			Texture.ReplaceTexture(Data.TextureUI);
			if (SelectedHighlight != null)
			{
				SelectedHighlight.SetActive(Data == Singleton<PlayerInfoScript>.Instance.SaveData.SelectedCardBack);
			}
		}
	}

	public override void Unload()
	{
		Texture.UnloadTexture();
	}

	public void OnClick()
	{
		if (Data != null)
		{
			Singleton<PlayerCustomizationController>.Instance.OnClickCardBackEntry(this);
		}
	}
}

using System.Collections.Generic;

public class PlayerPortraitData : ILoadableData, UnlockableData
{
	public string ID { get; private set; }

	public string Name { get; private set; }

	public string Texture { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Texture = TFUtils.LoadString(dict, "Texture", string.Empty);
	}

	public void ApplyTexture(UITexture textureObj)
	{
		if (textureObj.ReferenceCountingEnabled())
		{
			textureObj.UnloadTexture();
		}
		else
		{
			textureObj.mainTexture = null;
		}
		if (ID == "Facebook")
		{
			textureObj.EnableReferenceCounting(false);
			textureObj.mainTexture = Singleton<PlayerInfoScript>.Instance.GetPlayerPortrait();
			return;
		}
		textureObj.EnableReferenceCounting(true);
		if (ID == "Default")
		{
			string portraitTexture = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader.SelectedSkin.PortraitTexture;
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(portraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", textureObj);
		}
		else
		{
			textureObj.ReplaceTexture(Texture);
		}
	}
}

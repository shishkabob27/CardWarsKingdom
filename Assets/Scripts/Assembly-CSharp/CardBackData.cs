using System.Collections.Generic;

public class CardBackData : ILoadableData, UnlockableData
{
	public string ID { get; private set; }

	public string Name { get; private set; }

	public string Texture3D { get; private set; }

	public string TextureUI { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Texture3D = "Textures/CardBacks/" + TFUtils.LoadString(dict, "3DTexture", string.Empty);
		TextureUI = "UI/CardFrames/" + TFUtils.LoadString(dict, "UITexture", string.Empty);
		if (ID == "Default")
		{
			CardBackDataManager.DefaultData = this;
		}
	}
}

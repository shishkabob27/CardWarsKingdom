using System.Collections.Generic;

public class ExpeditionNameData : ILoadableData
{
	public string ID { get; private set; }

	public string Name { get; private set; }

	public string Texture { get; private set; }

	public string TextureAssetBundle { get; private set; }

	public int Difficulty { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		TextureAssetBundle = TFUtils.LoadString(dict, "TextureAssetBundle", null);
		string text = ((TextureAssetBundle == null) ? "UI/Icons_ActionPortraits/" : ("UI/Icons_ActionPortraits/" + TextureAssetBundle + "/"));
		Texture = text + TFUtils.LoadString(dict, "Texture", string.Empty);
		Difficulty = TFUtils.LoadInt(dict, "Difficulty", -1);
	}
}

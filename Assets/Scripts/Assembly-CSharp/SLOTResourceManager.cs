using System.Collections.Generic;

public class SLOTResourceManager : Singleton<SLOTResourceManager>
{
	public List<string> BundlesToLoadUpFront;
	public string resourcePathPrefix_hires;
	public string resourcePathPrefix_lores;
	public List<UIAtlas> UiAtlases;
	public UIAtlas lowResAtlas;
}

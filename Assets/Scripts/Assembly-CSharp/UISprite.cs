using UnityEngine;

public class UISprite : UIBasicSprite
{
	[SerializeField]
	private UIAtlas mAtlas;
	[SerializeField]
	private string mSpriteName;
	[SerializeField]
	private bool mFillCenter;
}

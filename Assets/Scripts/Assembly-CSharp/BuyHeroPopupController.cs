using UnityEngine;

public class BuyHeroPopupController : Singleton<BuyHeroPopupController>
{
	public UITweenController ShowTween;

	public UILabel Name;

	public UILabel NameShadow;

	public UILabel Quote;

	public UITexture Image;

	private void Awake()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			base.gameObject.ChangeLayer(LayerMask.NameToLayer("TopGUI"));
		}
	}

	public void Show(LeaderData leader)
	{
		ShowTween.Play();
		Name.text = leader.Name;
		NameShadow.text = Name.text;
		Quote.text = leader.FlvQuote;
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(leader.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", Image);
	}

	public void Unload()
	{
		Image.UnloadTexture();
	}
}

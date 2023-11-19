using UnityEngine;

public class MailController : Singleton<MailController>
{
	public float CurrencyOffset;
	public GameObject MailPrefab;
	public UITweenController GainCurrencyTween;
	public UITweenController GainCurrencyFinishTween;
	public UITweenController ItemGainedTween;
	public UIStreamingGrid MailGrid;
	public GameObject Information;
	public GameObject Prompt;
	public UILabel TitleLabel;
	public UILabel BodyLabel;
	public Transform BodyTextParent;
	public GameObject BodyCurrencyParent;
	public UISprite GainedCurrencySprite;
	public UILabel GainedCurrencyLabel;
	public UIPanel BodyPanel;
	public GameObject RewardedItemNode;
	public UILabel RewardedItemAmount;
}

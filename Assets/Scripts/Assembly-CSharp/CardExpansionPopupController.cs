using UnityEngine;

public class CardExpansionPopupController : Singleton<CardExpansionPopupController>
{
	public UITweenController ShowTween;
	public UITweenController HideTween;
	public UILabel Body;
	public UITexture EvoMaterialTexture;
	public UILabel EvoMaterialAmount;
	public UILabel SoftCurrencyAmount;
	public UITexture OwnedEvoMaterialTexture;
	public UILabel OwnedEvoMaterialAmount;
	public UILabel OwnedSoftCurrencyAmount;
	public GameObject YesNoGroup;
	public GameObject OkGroup;
}

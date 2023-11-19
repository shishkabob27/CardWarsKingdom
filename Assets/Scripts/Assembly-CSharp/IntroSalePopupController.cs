using UnityEngine;

public class IntroSalePopupController : Singleton<IntroSalePopupController>
{
	public UITweenController Mode1ShowTween;
	public UITweenController Mode1HideTween;
	public UITweenController Mode2ShowTween;
	public UITweenController Mode2HideTween;
	public GameObject Mode1Panel;
	public GameObject Mode2Panel;
	public UILabel Mode1HeaderLabel;
	public UILabel Mode1HardCurrencyLabel;
	public UILabel Mode1HardCurrencyLabelShadow;
	public UILabel Mode1SocialCurrencyLabel;
	public UILabel Mode1SoftCurrencyLabel;
	public UILabel Mode1PriceLabel;
	public UILabel Mode1PriceLabelShadow;
	public UILabel Mode2HardCurrencyLabel;
	public UILabel Mode2SocialCurrencyLabel;
	public UILabel Mode2SoftCurrencyLabel;
	public UILabel Mode2PriceLabel;
	public UILabel Mode2PriceLabelShadow;
	public UILabel Mode2PercentLabel;
	public UILabel Mode2PercentLabelShadow;
}

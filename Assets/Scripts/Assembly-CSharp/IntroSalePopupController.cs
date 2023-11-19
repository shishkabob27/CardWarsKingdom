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

	private StoreScreenController.FoundProductData mIntroPackage;

	private bool mMode1;

	public int GetTestMode()
	{
		return mMode1 ? 1 : 2;
	}

	public void Show(StoreScreenController.FoundProductData introPackage)
	{
		mIntroPackage = introPackage;
		if (Random.Range(0, 2) == 0)
		{
			mMode1 = true;
			Mode1ShowTween.Play();
			Mode1Panel.SetActive(true);
			Mode2Panel.SetActive(false);
			Mode1HeaderLabel.text = mIntroPackage.PackageData.PaidHardCurrency.ToString();
			Mode1HardCurrencyLabel.text = mIntroPackage.PackageData.FreeHardCurrency.ToString();
			Mode1HardCurrencyLabelShadow.text = Mode1HardCurrencyLabel.text;
			Mode1SocialCurrencyLabel.text = mIntroPackage.PackageData.SocialCurrency.ToString();
			Mode1SoftCurrencyLabel.text = mIntroPackage.PackageData.SoftCurrency.ToString();
			Mode1PriceLabel.text = introPackage.ProductData.FormattedPrice;
			Mode1PriceLabelShadow.text = introPackage.ProductData.FormattedPrice;
		}
		else
		{
			mMode1 = false;
			Mode2ShowTween.Play();
			Mode1Panel.SetActive(false);
			Mode2Panel.SetActive(true);
			int num = 100 * mIntroPackage.PackageData.TotalHardCurrency / mIntroPackage.PackageData.PaidHardCurrency;
			Mode2PercentLabel.text = "A " + num + "% Value!";
			Mode2PercentLabelShadow.text = Mode2PercentLabel.text;
			Mode2HardCurrencyLabel.text = mIntroPackage.PackageData.TotalHardCurrency.ToString();
			Mode2SocialCurrencyLabel.text = mIntroPackage.PackageData.SocialCurrency.ToString();
			Mode2SoftCurrencyLabel.text = mIntroPackage.PackageData.SoftCurrency.ToString();
			Mode2PriceLabel.text = introPackage.ProductData.FormattedPrice;
			Mode2PriceLabelShadow.text = introPackage.ProductData.FormattedPrice;
		}
	}

	public void OnClickAccept()
	{
		Singleton<StoreScreenController>.Instance.OnEntryClicked(mIntroPackage);
		Close();
	}

	public void OnClickDecline()
	{
		Close();
	}

	private void Close()
	{
		if (mMode1)
		{
			Mode1HideTween.Play();
		}
		else
		{
			Mode2HideTween.Play();
		}
		if (Singleton<TutorialController>.Instance.IsBlockActive("Bank"))
		{
			Singleton<TutorialController>.Instance.AdvanceTutorialState();
		}
	}
}

using UnityEngine;

public class CardExpansionPopupController : Singleton<CardExpansionPopupController>
{
	public delegate void PopupButtonCallback();

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

	private PopupButtonCallback mYesCallback;

	public void ShowPrompt(string body, string evoMaterialTexture, int evoMaterialAmount, int softCurrencyAmount, int ownedEvoMaterialAmount, PopupButtonCallback yesCallback)
	{
		YesNoGroup.SetActive(true);
		OkGroup.SetActive(false);
		mYesCallback = yesCallback;
		Body.text = body;
		EvoMaterialTexture.ReplaceTexture(evoMaterialTexture);
		EvoMaterialAmount.text = "x" + evoMaterialAmount;
		SoftCurrencyAmount.text = softCurrencyAmount.ToString();
		OwnedEvoMaterialTexture.ReplaceTexture(evoMaterialTexture);
		OwnedEvoMaterialAmount.text = "x" + ownedEvoMaterialAmount;
		OwnedSoftCurrencyAmount.text = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
		ShowTween.Play();
	}

	public void ShowMessage(string body, string evoMaterialTexture, int evoMaterialAmount, int softCurrencyAmount, int ownedEvoMaterialAmount)
	{
		YesNoGroup.SetActive(false);
		OkGroup.SetActive(true);
		Body.text = body;
		EvoMaterialTexture.ReplaceTexture(evoMaterialTexture);
		EvoMaterialAmount.text = "x" + evoMaterialAmount;
		SoftCurrencyAmount.text = softCurrencyAmount.ToString();
		OwnedEvoMaterialTexture.ReplaceTexture(evoMaterialTexture);
		OwnedEvoMaterialAmount.text = "x" + ownedEvoMaterialAmount;
		OwnedSoftCurrencyAmount.text = Singleton<PlayerInfoScript>.Instance.SaveData.SoftCurrency.ToString();
		ShowTween.Play();
	}

	public void OnClickYes()
	{
		HideTween.Play();
		PopupButtonCallback popupButtonCallback = mYesCallback;
		mYesCallback = null;
		popupButtonCallback();
	}

	public void OnClickNo()
	{
		HideTween.Play();
	}
}

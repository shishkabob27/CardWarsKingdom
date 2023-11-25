using System.Collections;
using UnityEngine;

public class BuyStaminaPopupController : Singleton<BuyStaminaPopupController>
{
	public float TickTime;

	public UITweenController ShowTween;

	public UILabel QuestStaminaLabel;

	public UILabel PvpStaminaLabel;

	private SimplePopupController.PopupButtonCallback mCallback;

	private void Awake()
	{
		if (DetachedSingleton<SceneFlowManager>.Instance.InBattleScene())
		{
			base.gameObject.ChangeLayer(LayerMask.NameToLayer("TopGUI"));
		}
	}

	public void Show(SimplePopupController.PopupButtonCallback callback = null)
	{
		ShowTween.Play();
		mCallback = callback;
		UICamera.LockInput();
		QuestStaminaLabel.text = "0";
		PvpStaminaLabel.text = "0";
	}

	public void OnShowTweenDone()
	{
		StartCoroutine(TickUpValues());
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/UI_BuyStamina");
	}

	private IEnumerator TickUpValues()
	{
		int questTarget = Singleton<PlayerInfoScript>.Instance.RankData.Stamina;
		int pvpTarget = MiscParams.MaxPvpStamina;
		float questTickRate = (float)questTarget / TickTime;
		float pvpTickRate = (float)pvpTarget / TickTime;
		float questCurrent = 0f;
		float pvpCurrent = 0f;
		while (questCurrent < (float)questTarget || pvpCurrent < (float)pvpTarget)
		{
			questCurrent = questCurrent.TickTowards(questTarget, questTickRate);
			pvpCurrent = pvpCurrent.TickTowards(pvpTarget, pvpTickRate);
			QuestStaminaLabel.text = ((int)questCurrent).ToString();
			PvpStaminaLabel.text = ((int)pvpCurrent).ToString();
			yield return null;
		}
		UICamera.UnlockInput();
	}

	public void OnClickOk()
	{
		SimplePopupController.PopupButtonCallback popupButtonCallback = mCallback;
		mCallback = null;
		if (popupButtonCallback != null)
		{
			popupButtonCallback();
		}
	}
}

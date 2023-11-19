using UnityEngine;

public class BuyInventoryPopupController : Singleton<BuyInventoryPopupController>
{
	public UITweenController ShowTween;

	public GameObject MainPanel;

	public UILabel IncreaseLabel;

	public UILabel TotalLabel;

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
		IncreaseLabel.text = KFFLocalization.Get("!!INVENTORY_INCREASEDBY").Replace("<val1>", MiscParams.InventorySpacePerPurchase.ToString());
		string newValue = Singleton<PlayerInfoScript>.Instance.SaveData.InventorySpace.ToString();
		if (Singleton<PlayerInfoScript>.Instance.SaveData.FilledInventoryCount > (int)Singleton<PlayerInfoScript>.Instance.SaveData.InventorySpace)
		{
			string newValue2 = Singleton<PlayerInfoScript>.Instance.SaveData.FilledInventoryCount.ToString();
			TotalLabel.text = KFFLocalization.Get("!!INVENTORY_TOTAL_NEED_MORE").Replace("<val1>", newValue).Replace("<val2>", newValue2);
		}
		else
		{
			TotalLabel.text = KFFLocalization.Get("!!INVENTORY_TOTAL").Replace("<val1>", newValue);
		}
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

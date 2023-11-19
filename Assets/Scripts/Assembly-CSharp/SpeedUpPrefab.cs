using System.Collections;
using UnityEngine;

public class SpeedUpPrefab : MonoBehaviour
{
	public UILabel TimeLabel;

	public UILabel Description;

	public UILabel OwnedCount;

	public UILabel Cost;

	public GameObject CostParent;

	public GameObject UseParent;

	private SpeedUpData mData;

	public void Populate(SpeedUpData data)
	{
		mData = data;
		TimeLabel.text = PlayerInfoScript.FormatTimeString(data.Minutes * 60, true, true);
		Description.text = KFFLocalization.Get("!!SPEEDUP_DESCRIPTION").Replace("<val1>", TimeLabel.text);
		int num = Singleton<PlayerInfoScript>.Instance.SpeedUpCount(data);
		if (num > 0)
		{
			CostParent.SetActive(false);
			UseParent.SetActive(true);
			OwnedCount.text = num + " " + KFFLocalization.Get("!!OWNED");
		}
		else
		{
			CostParent.SetActive(true);
			UseParent.SetActive(false);
			OwnedCount.text = string.Empty;
			Cost.text = data.Price.ToString();
		}
	}

	public void PressButton()
	{
		StartCoroutine(PressButtonCo());
	}

	private IEnumerator PressButtonCo()
	{
		int selection = -1;
		int ownedCount = Singleton<PlayerInfoScript>.Instance.SpeedUpCount(mData);
		if (ownedCount > 0)
		{
			string message = KFFLocalization.Get("!!SPEEDUP_USE_CONFIRM").Replace("<val1>", TimeLabel.text);
			Singleton<SimplePopupController>.Instance.ShowPrompt(string.Empty, message, delegate
			{
				selection = 1;
			}, delegate
			{
				selection = 0;
			});
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowPurchasePrompt(KFFLocalization.Get("!!SPEEDUP_BUY_CONFIRM").Replace("<val2>", TimeLabel.text), KFFLocalization.Get("!!SPEEDUP_NOBUY_CONFIRM"), mData.Price, delegate
			{
				selection = 1;
			}, delegate
			{
				selection = 0;
			});
		}
		while (true)
		{
			switch (selection)
			{
			case -1:
				yield return null;
				continue;
			case 0:
				yield break;
			}
			if (ownedCount == 0)
			{
				int outcome = -1;
				StartCoroutine(Singleton<PlayerInfoScript>.Instance.ConsumeHardCurrencyCo(mData.Price, mData.ID, delegate(bool success)
				{
					outcome = (success ? 1 : 0);
				}));
				while (true)
				{
					switch (outcome)
					{
					case -1:
						yield return null;
						continue;
					case 0:
						yield break;
					}
					break;
				}
			}
			else
			{
				Singleton<PlayerInfoScript>.Instance.ConsumeSpeedUp(mData);
			}
			Singleton<UseSpeedUpsPopup>.Instance.ApplySpeedup(mData);
			yield break;
		}
	}
}

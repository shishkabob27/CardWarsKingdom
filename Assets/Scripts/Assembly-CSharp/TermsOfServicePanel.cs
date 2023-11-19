using System;
using UnityEngine;

public class TermsOfServicePanel : MonoBehaviour
{
	public GameObject OkButton;

	private string LinkPrivacy = "http://www.cartoonnetwork.com/legal/priv_tou.html#textBox_priv_a";

	private string LinkTerms = "http://www.cartoonnetwork.com/legal/priv_tou.html#textBox_priv_a";

	private Action mOnConfirmed;

	private bool mChecked;

	public void Show(Action onConfirmed)
	{
		mOnConfirmed = onConfirmed;
		base.gameObject.SetActive(true);
	}

	public void OnClickOk()
	{
		mOnConfirmed();
	}

	public void OpenPrivacyPolicy()
	{
		if (testInternetConnection())
		{
			string text = KFFLocalization.Get("!!PRIVACY_LINK");
			if (string.IsNullOrEmpty(text))
			{
				text = LinkPrivacy;
			}
			Application.OpenURL(text);
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public void OpenTermsOfUse()
	{
		if (testInternetConnection())
		{
			string text = KFFLocalization.Get("!!TERMSOFUSE_LINK");
			if (string.IsNullOrEmpty(text))
			{
				text = LinkTerms;
			}
			Application.OpenURL(text);
		}
		else
		{
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!CONNECTIONFAILED"));
		}
	}

	public static bool testInternetConnection()
	{
		bool flag = false;
		if (Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork && Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			return false;
		}
		return true;
	}
}

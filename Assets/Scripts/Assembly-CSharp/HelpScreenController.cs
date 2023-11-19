using System.Collections.Generic;
using UnityEngine;

public class HelpScreenController : Singleton<HelpScreenController>
{
	public GameObject HelpEntryPrefab;

	public UITweenController HelpPopupTween;

	public UIStreamingGrid HelpList;

	private UIStreamingGridDataSource<HelpEntry> mHelpListDataSource = new UIStreamingGridDataSource<HelpEntry>();

	public UILabel HelpTitle;

	public UILabel HelpBody;

	public void Populate()
	{
		mHelpListDataSource.Init(HelpList, HelpEntryPrefab, HelpDataManager.Instance.GetDatabase());
	}

	public void OnEntryClicked(HelpEntry entry)
	{
		HelpPopupTween.Play();
		HelpTitle.text = entry.Title;
		HelpBody.text = entry.Body;
		HelpKPITrack(entry.Title);
	}

	public void OnCustomerSupportClick()
	{
		Singleton<HelpShiftManager>.Instance.ShowConversation(HelpshiftConfig.Instance.getApiConfig());
	}

	public void OnFAQClick()
	{
		Singleton<HelpShiftManager>.Instance.ShowFAQ();
	}

	private void HelpKPITrack(string topic)
	{
		string upsightEvent = "Help.HelpTopic";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("topic", topic);
		Singleton<KFFUpsightVGController>.Instance.RecordCustomEvent(upsightEvent, dictionary);
	}
}

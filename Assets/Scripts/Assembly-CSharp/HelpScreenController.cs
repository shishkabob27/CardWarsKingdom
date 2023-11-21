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
	}
}

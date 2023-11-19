public class HelpListPrefab : UIStreamingGridListItem
{
	public UILabel Title;

	private HelpEntry mEntry;

	public override void Populate(object dataObj)
	{
		mEntry = dataObj as HelpEntry;
		Title.text = mEntry.Title;
	}

	private void OnClick()
	{
		Singleton<HelpScreenController>.Instance.OnEntryClicked(mEntry);
	}
}

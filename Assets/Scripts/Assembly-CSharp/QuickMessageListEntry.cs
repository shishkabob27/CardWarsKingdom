public class QuickMessageListEntry : UIStreamingGridListItem
{
	public UILabel TextLabel;

	private QuickChatData mData;

	public override void Populate(object dataObj)
	{
		mData = dataObj as QuickChatData;
		TextLabel.text = mData.Text;
	}

	public void OnClick()
	{
		Singleton<QuickMessageController>.Instance.OnClickChatEntry(mData);
	}
}

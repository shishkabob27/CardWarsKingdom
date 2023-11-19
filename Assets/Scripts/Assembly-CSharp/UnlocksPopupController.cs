public class UnlocksPopupController : Singleton<UnlocksPopupController>
{
	public delegate void UnlockDoneCallback();

	public UITweenController UnlockPortraitTween;

	public UILabel UnlockedPortraitName;

	public UITexture UnlockedPortraitTexture;

	public UITweenController UnlockCardBackTween;

	public UILabel UnlockedCardBackName;

	public UITexture UnlockedCardBackTexture;

	private UnlockDoneCallback mUnlockDoneCallback;

	public void ShowUnlock(UnlockableData unlock, UnlockDoneCallback doneCallback)
	{
		mUnlockDoneCallback = doneCallback;
		if (unlock is PlayerPortraitData)
		{
			UnlockPortraitTween.Play();
			PlayerPortraitData playerPortraitData = unlock as PlayerPortraitData;
			UnlockedPortraitName.text = playerPortraitData.Name;
			UnlockedPortraitTexture.ReplaceTexture(playerPortraitData.Texture);
		}
		else if (unlock is CardBackData)
		{
			UnlockCardBackTween.Play();
			CardBackData cardBackData = unlock as CardBackData;
			UnlockedCardBackName.text = cardBackData.Name;
			UnlockedCardBackTexture.ReplaceTexture(cardBackData.TextureUI);
		}
		else
		{
			mUnlockDoneCallback();
		}
	}

	public void OnPopupClosed()
	{
		mUnlockDoneCallback();
	}
}

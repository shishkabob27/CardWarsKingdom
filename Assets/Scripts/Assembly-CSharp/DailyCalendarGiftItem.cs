using UnityEngine;

public class DailyCalendarGiftItem : MonoBehaviour
{
	public UILabel DayLabel;

	public UISprite RewardIcon;

	public UITexture RewardTexture;

	public GameObject RewardFX;

	public UITweenController ClaimedTween;

	public GameObject ClaimedRoot;

	public UISprite Fade;

	public UISprite RewardIconBorder;

	public void Claim()
	{
		RewardFX.SetActive(true);
		ClaimedTween.Play();
	}

	public void SetClaimed(bool claimed)
	{
		ClaimedRoot.SetActive(claimed);
		Fade.gameObject.SetActive(claimed);
	}
}

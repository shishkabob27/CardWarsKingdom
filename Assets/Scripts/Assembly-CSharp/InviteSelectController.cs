using System;
using Multiplayer;

public class InviteSelectController : Singleton<InviteSelectController>
{
	public UITweenController ShowTween;

	public UILabel CodeLabel;

	public void Show()
	{
		ShowTween.Play();
		CodeLabel.text = global::Multiplayer.Multiplayer.GenerateRedeemCode(Singleton<PlayerInfoScript>.Instance.GetPlayerCode());
	}

	private void Update()
	{
	}

	public void OnClickSendEmail()
	{
		EMailer.PrefillAndShow(null, RedemptionCodeMessager.GenerateMessageSubject(), RedemptionCodeMessager.GenerateLongMessageBody());
	}

	public void OnClickSendFacebook()
	{
		FacebookPoster.PrefillAndShow(RedemptionCodeMessager.GenerateMessageSubject(), RedemptionCodeMessager.GenerateLongMessageSubject(), RedemptionCodeMessager.GenerateTweetMessage());
	}

	public void OnClickSendTweet()
	{
		Twitterer.Tweet(RedemptionCodeMessager.GenerateTweetMessage());
	}
}

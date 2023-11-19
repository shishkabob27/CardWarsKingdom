using Multiplayer;
using UnityEngine;

public class RedemptionCodeMessager
{
	public static string GenerateMessageSubject()
	{
		return Language.Get("!!PLAY_WITH_ME");
	}

	public static string GenerateLongMessageSubject()
	{
		return Language.Get("!!PLAY_WITH_ME_DESCRIPTION");
	}

	public static string GenerateLongMessageBody()
	{
		string iosUpdateUrl = SessionManager.Instance.theSession.IosUpdateUrl;
		string androidUpdateUrl = SessionManager.Instance.theSession.AndroidUpdateUrl;
		return string.Format(Language.Get("!!REDEMPTION_EMAIL_MESSAGE"), PlayerCode(), iosUpdateUrl, androidUpdateUrl);
	}

	public static string GenerateTweetMessage()
	{
		UnityEngine.Debug.Log(Language.Get("!!REDEMPTION_TWEET_MESSAGE"));
		return string.Format(Language.Get("!!REDEMPTION_TWEET_MESSAGE"), PlayerCode());
	}

	private static string PlayerCode()
	{
		return global::Multiplayer.Multiplayer.GenerateRedeemCode(Singleton<PlayerInfoScript>.Instance.GetPlayerCode());
	}
}

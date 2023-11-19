public interface ISocialGamingNetworkListener
{
	void OnEnable();

	void OnDisable();

	bool IsAuthenticated();

	void ReportAchievement(string aID, float aPercent);

	void ShowAchievements();

	void ShowBannerAchievement();
}

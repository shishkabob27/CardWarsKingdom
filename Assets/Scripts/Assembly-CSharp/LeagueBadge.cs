using UnityEngine;

public class LeagueBadge : MonoBehaviour
{
	public UITexture PortraitTexture;

	public UITexture CurrentLeagueTexture;

	public UILabel NameLabel;

	public UILabel RankNumberLabel;

	public UILabel RankNameLabel;

	public UITexture FlagTexture;

	[SerializeField]
	private Color[] _RankLabelColors = new Color[0];

	[SerializeField]
	private Color[] _RankLabelOutlineColors = new Color[0];

	[SerializeField]
	private Color[] _RankLabelShadowColors = new Color[0];

	[SerializeField]
	private Color[] _RankLabelShadowOutlineColors = new Color[0];

	public void PopulateMyData(int overrideCurrentLeague = -1, int overrideBestLeague = -1)
	{
		string playerName = Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName;
		int currentLeague = ((overrideCurrentLeague == -1) ? Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerLevel : overrideCurrentLeague);
		int bestLeague = ((overrideBestLeague == -1) ? ((int)Singleton<PlayerInfoScript>.Instance.SaveData.BestMultiplayerLevel) : overrideBestLeague);
		string myFlag = Singleton<CountryFlagManager>.Instance.GetMyFlag();
		if (PortraitTexture != null)
		{
			if (PortraitTexture.ReferenceCountingEnabled())
			{
				PortraitTexture.UnloadTexture();
			}
			else
			{
				PortraitTexture.mainTexture = null;
			}
			PortraitTexture.EnableReferenceCounting(true);
			Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait.ApplyTexture(PortraitTexture);
		}
		PopulateTheRest(playerName, currentLeague, bestLeague, myFlag);
	}

	public void PopulateOtherPlayerData(string playerName, string portraitTexturePath, int currentLeague, int bestLeague, string flag)
	{
		if (PortraitTexture != null)
		{
			if (PortraitTexture.ReferenceCountingEnabled())
			{
				PortraitTexture.UnloadTexture();
			}
			else
			{
				PortraitTexture.mainTexture = null;
			}
			PortraitTexture.EnableReferenceCounting(true);
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(portraitTexturePath, "FTUEBundle", "UI/UI/LoadingPlaceholder", PortraitTexture);
		}
		PopulateTheRest(playerName, currentLeague, bestLeague, flag);
	}

	public void PopulateOtherPlayerData(string playerName, Texture facebookPortrait, int currentLeague, int bestLeague, string flag)
	{
		PopulateFacebookPortrait(facebookPortrait);
		PopulateTheRest(playerName, currentLeague, bestLeague, flag);
	}

	public void PopulateFacebookPortrait(Texture facebookPortrait)
	{
		if (PortraitTexture != null)
		{
			if (PortraitTexture.ReferenceCountingEnabled())
			{
				PortraitTexture.UnloadTexture();
			}
			else
			{
				PortraitTexture.mainTexture = null;
			}
			PortraitTexture.EnableReferenceCounting(false);
			PortraitTexture.mainTexture = facebookPortrait;
		}
	}

	public void Populate(object rankDataObj)
	{
		PvpRankData pvpRankData = rankDataObj as PvpRankData;
		PvpSeasonData activePvpSeason = Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason;
		string playerName = PvpSeasonDataManager.Instance.RankName(pvpRankData.Rank);
		if (PortraitTexture != null)
		{
			if (PortraitTexture.ReferenceCountingEnabled())
			{
				PortraitTexture.UnloadTexture();
			}
			else
			{
				PortraitTexture.mainTexture = null;
			}
			PortraitTexture.EnableReferenceCounting(true);
			Singleton<PlayerInfoScript>.Instance.SaveData.SelectedPortrait.ApplyTexture(PortraitTexture);
		}
		PopulateTheRest(playerName, pvpRankData.Rank, 0, null);
	}

	private void PopulateTheRest(string playerName, int currentLeague, int bestLeague, string flag)
	{
		PvpRankData rank = PvpRankDataManager.Instance.GetRank(currentLeague);
		if (NameLabel != null)
		{
			NameLabel.text = playerName;
		}
		if (RankNameLabel != null)
		{
			string inRankTextureName = "Rank_Unranked";
			string text = KFFLocalization.Get("!!UNRANKED");
			if (rank != null && currentLeague != -1)
			{
				text = rank.ShortRankName;
				inRankTextureName = rank.LeagueTextureNameOnly;
			}
			RankNameLabel.text = text;
			ColorRankNameLabel(inRankTextureName);
		}
		if (RankNumberLabel != null)
		{
			RankNumberLabel.text = ((currentLeague != -1) ? currentLeague.ToString() : string.Empty);
		}
		if (CurrentLeagueTexture != null)
		{
			string newTexturePath = "UI/Icons_Leagues/Rank_Unranked";
			if (rank != null && currentLeague != -1)
			{
				newTexturePath = rank.LeagueTexture;
			}
			CurrentLeagueTexture.ReplaceTexture(newTexturePath);
		}
	}

	public void ShowNameAndFlag(bool show)
	{
		if (NameLabel != null)
		{
			NameLabel.alpha = (show ? 1 : 0);
		}
	}

	private void ColorRankNameLabel(string inRankTextureName)
	{
		int num = 0;
		switch (inRankTextureName)
		{
		case "Rank_Unranked":
			num = 0;
			break;
		case "Rank01_Amadeus":
			num = 1;
			break;
		case "Rank02_Legend":
			num = 2;
			break;
		case "Rank03_Master":
			num = 3;
			break;
		case "Rank04_Virtuoso":
			num = 4;
			break;
		case "Rank05_Wizard":
			num = 5;
			break;
		case "Rank06_Genius":
			num = 6;
			break;
		case "Rank07_Warrior":
			num = 7;
			break;
		case "Rank08_CoolGuy":
			num = 8;
			break;
		case "Rank09_Dweeb":
			num = 9;
			break;
		}
		RankNameLabel.color = _RankLabelColors[num];
		RankNameLabel.effectColor = _RankLabelOutlineColors[num];
		if (RankNameLabel.LabelShadow != null)
		{
			RankNameLabel.LabelShadow.ShadowTextColor = _RankLabelShadowColors[num];
			RankNameLabel.LabelShadow.ShadowEffectColor = _RankLabelShadowOutlineColors[num];
			RankNameLabel.LabelShadow.RefreshShadowLabel();
		}
	}
}

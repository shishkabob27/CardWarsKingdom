using UnityEngine;

public class SampleExpeditionRewardsData
{
	public string TierString;

	public Color[] ColorPalette;

	public int[] RarityRange;

	public int MaxSoftCurrency;

	public int MaxHardCurrency;

	public bool ShouldShowRankXP;

	public bool ShouldShowShardTexture;

	public bool ShouldShowCakeTexture;

	public bool ShouldShowSpeedUpsTexture;

	public SampleExpeditionRewardsData(string inTierString, Color[] inColorPalette, int inMinRarity, int inMaxRarity, int inMaxSoftCurrency, int inMaxHardCurrency, bool inShouldShowRankXP, bool inShowShardTexture, bool inShowCakeTexture, bool inShowSpeedUpsTexture)
	{
		RarityRange = new int[2] { inMinRarity, inMaxRarity };
		TierString = inTierString;
		ColorPalette = inColorPalette;
		MaxSoftCurrency = inMaxSoftCurrency;
		MaxHardCurrency = inMaxHardCurrency;
		ShouldShowRankXP = inShouldShowRankXP;
		ShouldShowShardTexture = inShowShardTexture;
		ShouldShowCakeTexture = inShowCakeTexture;
		ShouldShowSpeedUpsTexture = inShowSpeedUpsTexture;
	}
}

using System;
using UnityEngine;

public class PromoBannerData
{
	public string TitleText = string.Empty;

	public Color TitleTextColor = Color.white;

	public Color TitleTextOutlineColor = Color.black;

	public int TitleTextWidth;

	public string DescriptionText = string.Empty;

	public Color DescriptionTextColor = Color.white;

	public Color DescriptionTextOutlineColor = Color.black;

	public int DescriptionTextWidth;

	public string CurrencyText = string.Empty;

	public Color CurrencyTextColor = Color.white;

	public Color CurrencyTextOutlineColor = Color.black;

	public int CurrencyTextWidth;

	public string ButtonText = string.Empty;

	public PromoCurrencyType CurrencyType = PromoCurrencyType.None;

	public PromoButtonPlacement ButtonPlacement = PromoButtonPlacement.None;

	public Color ButtonColor = Color.green;

	public string BgTextureName;

	public Action OnClickedDelegate;

	public PromoBannerItemType BannerType;

	public DateTime BannerEndDate;

	public bool ShouldLocalizeText;

	public string BannerID;
}

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TownScheduleData : ILoadableData
{
	public string ID
	{
		get
		{
			return StartDate.ToString();
		}
	}

	public int StartDate { get; private set; }

	public string TownPrefab { get; private set; }

	public string BannerSprite { get; private set; }

	public Color BadgeColor { get; private set; }

	public Color BannerTextColor { get; private set; }

	public Color BannerTextOutlineColor { get; private set; }

	public Color BannerTextShadowColor { get; private set; }

	public Color BannerTextShadowOutlineColor { get; private set; }

	public string ChatIconSprite { get; private set; }

	public Color ChatWindowFrameColor { get; private set; }

	public Color ChatWindowFillColor { get; private set; }

	public string ChatMessageBgSprite { get; private set; }

	public string DeepLinkButtonBgSprite { get; private set; }

	public string SingleGachaChestAnim { get; private set; }

	public string MultiGachaChestAnim { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		StartDate = DateTime.ParseExact(TFUtils.LoadString(dict, "StartDate", string.Empty), "M/d", CultureInfo.GetCultureInfo("en-US")).DayOfYear;
		TownPrefab = TFUtils.LoadString(dict, "TownPrefab", string.Empty);
		BannerSprite = TFUtils.LoadString(dict, "BannerSprite", string.Empty);
		BadgeColor = TFUtils.LoadString(dict, "BadgeColor", string.Empty).ToColor();
		BannerTextColor = TFUtils.LoadString(dict, "BannerTextColor", string.Empty).ToColor();
		BannerTextOutlineColor = TFUtils.LoadString(dict, "BannerTextOutlineColor", string.Empty).ToColor();
		BannerTextShadowColor = TFUtils.LoadString(dict, "BannerTextShadowColor", string.Empty).ToColor();
		BannerTextShadowOutlineColor = TFUtils.LoadString(dict, "BannerTextShadowOutlineColor", string.Empty).ToColor();
		ChatIconSprite = TFUtils.LoadString(dict, "ChatIconSprite", string.Empty);
		ChatWindowFrameColor = TFUtils.LoadString(dict, "ChatWindowFrameColor", string.Empty).ToColor();
		ChatWindowFillColor = TFUtils.LoadString(dict, "ChatWindowFillColor", string.Empty).ToColor();
		ChatMessageBgSprite = TFUtils.LoadString(dict, "ChatMessageBgSprite", string.Empty);
		DeepLinkButtonBgSprite = TFUtils.LoadString(dict, "DeepLinkButtonBgSprite", string.Empty);
		SingleGachaChestAnim = TFUtils.LoadString(dict, "SingleGachaChestAnim", string.Empty);
		MultiGachaChestAnim = TFUtils.LoadString(dict, "MultiGachaChestAnim", string.Empty);
	}
}

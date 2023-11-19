using UnityEngine;

public class HeroSkinListEntry : UIStreamingGridListItem
{
	public GameObject Highlight;

	public UITexture LeaderImage;

	public GameObject OwnedObject;

	public UILabel CostLabel;

	public LeaderData Leader { get; set; }

	public override void Populate(object dataObj)
	{
		Leader = dataObj as LeaderData;
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(Leader.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", LeaderImage);
		LeaderItem leader = Singleton<PlayerInfoScript>.Instance.GetCurrentLoadout().Leader;
		bool active = leader.SelectedSkin == Leader;
		bool flag = true;
		if (Leader.SkinParentLeader != null)
		{
			LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(Leader.SkinParentLeader);
			if (!leaderItem.OwnedSkins.Contains(Leader))
			{
				flag = false;
			}
		}
		OwnedObject.SetActive(flag);
		Highlight.SetActive(active);
		if (!flag)
		{
			CostLabel.transform.SetParentActive(true);
			CostLabel.text = Leader.SkinBuyCost.ToString();
		}
		else
		{
			CostLabel.transform.SetParentActive(false);
		}
	}

	public override void Unload()
	{
		LeaderImage.UnloadTexture();
	}

	private void OnClick()
	{
		Singleton<PlayerCustomizationController>.Instance.OnClickHeroSkinEntry(Leader);
	}
}

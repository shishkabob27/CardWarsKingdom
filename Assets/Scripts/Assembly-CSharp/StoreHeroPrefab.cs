using UnityEngine;

public class StoreHeroPrefab : UIStreamingGridListItem
{
	public GameObject Highlight;

	public GameObject NormalFrame;

	public UITexture LeaderImage;

	public GameObject OwnedObject;

	public GameObject LockedObject;

	public LeaderData Leader { get; set; }

	public override void Populate(object dataObj)
	{
		Leader = dataObj as LeaderData;
		if (Leader != null)
		{
			Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(Leader.PortraitTexture, "FTUEBundle", "UI/UI/LoadingPlaceholder", LeaderImage);
			if (Leader.SkinParentLeader != null)
			{
				if (Highlight != null)
				{
					Highlight.SetActive(Singleton<StoreScreenController>.Instance.IsSkinHighlighted(Leader));
					NormalFrame.SetActive(!Singleton<StoreScreenController>.Instance.IsSkinHighlighted(Leader));
				}
				LeaderItem leaderItem = Singleton<PlayerInfoScript>.Instance.GetLeaderItem(Leader.SkinParentLeader);
				if (leaderItem != null)
				{
					if (LockedObject != null)
					{
						LockedObject.SetActive(false);
					}
					if (OwnedObject != null)
					{
						OwnedObject.SetActive(leaderItem.OwnedSkins.Contains(Leader));
					}
				}
				else
				{
					if (LockedObject != null)
					{
						LockedObject.SetActive(true);
					}
					if (OwnedObject != null)
					{
						OwnedObject.SetActive(false);
					}
				}
			}
			else
			{
				if (Highlight != null)
				{
					Highlight.SetActive(Singleton<StoreScreenController>.Instance.IsHeroHighlighted(Leader));
				}
				if (LockedObject != null)
				{
					LockedObject.SetActive(false);
				}
				if (OwnedObject != null)
				{
					OwnedObject.SetActive(Singleton<PlayerInfoScript>.Instance.IsLeaderUnlocked(Leader));
				}
			}
		}
		else
		{
			if (LeaderImage != null)
			{
				LeaderImage.ReplaceTexture("UI/UI/UI_Leader_0");
			}
			if (OwnedObject != null)
			{
				OwnedObject.SetActive(false);
			}
			if (LockedObject != null)
			{
				LockedObject.SetActive(false);
			}
			if (Highlight != null)
			{
				Highlight.SetActive(false);
			}
		}
	}

	public override void Unload()
	{
		if (LeaderImage != null)
		{
			LeaderImage.UnloadTexture();
		}
	}

	private void OnClick()
	{
		if (Leader != null)
		{
			if (Leader.SkinParentLeader != null)
			{
				Singleton<StoreScreenController>.Instance.OnSkinClicked(this);
			}
			else
			{
				Singleton<StoreScreenController>.Instance.OnLeaderClicked(this);
			}
		}
	}
}

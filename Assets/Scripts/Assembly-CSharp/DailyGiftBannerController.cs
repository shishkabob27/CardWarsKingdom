using System.Collections;
using UnityEngine;

public class DailyGiftBannerController : Singleton<DailyGiftBannerController>
{
	public UITweenController ShowBanner;

	public UITweenController HideBanner;

	public UILabel RewardLabel;

	public UISprite RewardIcon;

	public Transform RewardSlotNode;

	public UITexture CardBackTexture;

	private bool mShowing;

	public void Init(string displayName, string spriteName)
	{
		RewardLabel.text = displayName;
		CardBackTexture.gameObject.SetActive(false);
		RewardIcon.gameObject.SetActive(true);
		RewardIcon.spriteName = spriteName;
	}

	public void Init(string displayName, InventorySlotItem slotItem)
	{
		RewardLabel.text = displayName;
		CardBackTexture.gameObject.SetActive(false);
		RewardIcon.gameObject.SetActive(false);
		InventoryTile component = RewardSlotNode.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
		component.gameObject.ChangeLayer(base.gameObject.layer);
		component.SetAsDisplayOnly();
		component.Populate(slotItem);
	}

	public void Init(string displayName, CardBackData cardBack)
	{
		RewardLabel.text = displayName;
		RewardIcon.gameObject.SetActive(false);
		CardBackTexture.gameObject.SetActive(true);
		CardBackTexture.ReplaceTexture(cardBack.TextureUI);
	}

	public float GetDelay()
	{
		float num = 5.5f;
		if (ShowBanner != null)
		{
			TweenPosition component = ShowBanner.gameObject.GetComponent<UITweener>().target.GetComponent<TweenPosition>();
			return (!(null != component)) ? num : (component.delay + 0.5f);
		}
		return num;
	}

	public void ShowBannerNow()
	{
		mShowing = true;
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/SFX_Calendar_ChestOpen");
		if (ShowBanner != null)
		{
			ShowBanner.Play();
		}
	}

	public IEnumerator ShowBannerNowCo()
	{
		ShowBannerNow();
		float timer = GetDelay();
		while (timer > 0f && mShowing)
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		if (mShowing)
		{
			HideBannerNow();
		}
	}

	public void HideBannerNow()
	{
		if (mShowing)
		{
			HideBanner.Play();
		}
		mShowing = false;
	}

	public void Unload()
	{
		RewardSlotNode.DestroyAllChildren();
		CardBackTexture.UnloadTexture();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaMultiResultController : Singleton<GachaMultiResultController>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowBlackFadeTween;

	public UITweenController SummaryFrameShowTween;

	public UITweenController SummaryFrameHideTween;

	public UIGrid DropsGrid;

	public UIWidget DropGridSizeKey;

	public UILabel SummaryLabel;

	public bool FastForward;

	public bool canClose;

	public UIButton CollectAllButton;

	public UILabel CollectAllLabel;

	private List<InventoryTile> mLoot = new List<InventoryTile>();

	public float LootRevealInterval = 0.5f;

	public bool Showing { get; private set; }

	public void ShowMultiEggPanel(List<InventorySlotItem> creatures)
	{
		Showing = true;
		FastForward = false;
		canClose = false;
		CollectAllButton.gameObject.SetActive(true);
		ShowTween.Play();
		PopulateLoot(creatures);
	}

	private void PopulateLoot(List<InventorySlotItem> creatures)
	{
		mLoot.Clear();
		InventoryTile.ClearDelegates(true);
		int count = creatures.Count;
		if (count > 30)
		{
			DropsGrid.maxPerLine = 7;
		}
		else if (count > 15)
		{
			DropsGrid.maxPerLine = 6;
		}
		else
		{
			DropsGrid.maxPerLine = 5;
		}
		foreach (InventorySlotItem creature in creatures)
		{
			InventoryTile component = DropsGrid.gameObject.InstantiateAsChild(Singleton<PrefabReferences>.Instance.InventoryTile).GetComponent<InventoryTile>();
			component.gameObject.ChangeLayer(DropsGrid.gameObject.layer);
			component.Populate(creature);
			component.SetLootMode(false);
			component.GetComponent<Collider>().enabled = false;
			component.RarityStarGroup.SetActive(false);
			component.HideRarityFrame();
			mLoot.Add(component);
		}
		DropsGrid.Reposition();
		StartCoroutine(RevealEggsCo());
	}

	public void TriggerRevealAllEggs()
	{
		StartCoroutine(RevealEggsCo());
	}

	private IEnumerator RevealEggsCo()
	{
		yield return new WaitForSeconds(1f);
		int numNew = 0;
		bool isNewCreature = false;
		float leftMostPos = 0f;
		float rightMostPos = 0f;
		float topMostPos = 0f;
		float bottomMostPos = 0f;
		foreach (InventoryTile egg2 in mLoot)
		{
			bool firstTimeCollected = egg2.InventoryItem.FirstTimeCollected;
			Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_GachaFlyToCam");
			float xPos2 = egg2.transform.localPosition.x * 2f;
			float yPos2 = egg2.transform.localPosition.y * 2f;
			float travelDist = Mathf.Abs(xPos2) + Mathf.Abs(yPos2);
			float distPop = 0f;
			if (travelDist != 0f)
			{
				distPop = 500f / travelDist;
			}
			xPos2 *= distPop;
			yPos2 *= distPop;
			egg2.FlyInStartPos.transform.localPosition = new Vector3(xPos2, yPos2, 0f);
			leftMostPos = Mathf.Min(leftMostPos, egg2.transform.localPosition.x);
			rightMostPos = Mathf.Max(rightMostPos, egg2.transform.localPosition.x);
			topMostPos = Mathf.Max(topMostPos, egg2.transform.localPosition.y);
			bottomMostPos = Mathf.Min(bottomMostPos, egg2.transform.localPosition.y);
			float zRotNeg = Random.Range(-60, -30);
			float zRotPos = Random.Range(60, 30);
			if ((float)Random.Range(0, 1) > 0.5f)
			{
				egg2.FlyInTweenRot.from = new Vector3(0f, 0f, zRotNeg);
			}
			else
			{
				egg2.FlyInTweenRot.from = new Vector3(0f, 0f, zRotPos);
			}
			egg2.RevealLootDetails();
			egg2.SetDepthOffset(100);
			if (FastForward)
			{
				if (firstTimeCollected)
				{
					numNew++;
				}
				egg2.FlyInTween.Play();
				egg2.FlyInTween.End();
				egg2.ShowRarityFrame();
				egg2.RarityStarGroup.SetActive(true);
				yield return new WaitForSeconds(0.05f);
			}
			else
			{
				egg2.FlyInTween.Play();
				if (firstTimeCollected)
				{
					numNew++;
					if (egg2.InventoryItem.SlotType == InventorySlotType.Creature)
					{
						isNewCreature = true;
					}
					egg2.RevealNewTile();
					egg2.NewTilePopTween.Play();
					yield return new WaitForSeconds(0.4f);
				}
				else
				{
					egg2.ShowRarityFrame();
					egg2.RarityStarGroup.SetActive(true);
					yield return new WaitForSeconds(0.2f);
				}
				if (egg2.OpenLoot(true))
				{
					while (Singleton<GachaOpenSequencer>.Instance.Showing)
					{
						yield return null;
					}
					egg2.HideNewTile();
					egg2.ShowRarityFrame();
				}
			}
			egg2.SetDepthOffset(-100);
		}
		canClose = true;
		foreach (InventoryTile egg in mLoot)
		{
			egg.GetComponent<Collider>().enabled = true;
		}
		string summaryString2 = KFFLocalization.Get("!!CHESTS_OPENED").Replace("<val1>", mLoot.Count.ToString());
		if (numNew > 0)
		{
			if (isNewCreature)
			{
				if (numNew > 0 && numNew < 2)
				{
					summaryString2 = summaryString2 + " - " + KFFLocalization.Get("!!NEW_CREATURE1");
				}
				else if (numNew > 0)
				{
					summaryString2 = summaryString2 + " - " + KFFLocalization.Get("!!NEW_CREATURES").Replace("<val1>", numNew.ToString());
				}
			}
			else if (numNew > 0 && numNew < 2)
			{
				summaryString2 = summaryString2 + " - " + KFFLocalization.Get("!!NEW_ITEM1");
			}
			else if (numNew > 0)
			{
				summaryString2 = summaryString2 + " - " + KFFLocalization.Get("!!NEW_ITEMS").Replace("<val1>", numNew.ToString());
			}
		}
		summaryString2 += "!";
		SummaryLabel.text = summaryString2;
		float cellWidth = DropsGrid.cellWidth;
		float cellHeight = DropsGrid.cellHeight;
		DropGridSizeKey.width = Mathf.RoundToInt(Mathf.Abs(leftMostPos) + Mathf.Abs(rightMostPos) + cellWidth);
		DropGridSizeKey.height = Mathf.RoundToInt(Mathf.Abs(topMostPos) + Mathf.Abs(bottomMostPos) + cellHeight);
		SummaryFrameShowTween.Play();
		if (!FastForward)
		{
			CollectAllNow();
		}
	}

	public void OnCloseMultiGachaPanel()
	{
		if (canClose)
		{
			HideTween.Play();
			SummaryFrameHideTween.Play();
			DropsGrid.transform.DestroyAllChildren();
			mLoot.Clear();
			Showing = false;
		}
	}

	public void CollectAllNow()
	{
		if (!FastForward)
		{
			FastForward = true;
			CollectAllButton.gameObject.SetActive(false);
		}
		else
		{
			OnCloseMultiGachaPanel();
		}
	}
}

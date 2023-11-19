using System.Collections;
using UnityEngine;

public class LabBuildingController : Singleton<LabBuildingController>
{
	public UITweenController HideTween;

	public GameObject CreatureEnhanceLock;

	public Collider CreatureEnhanceCol;

	public GameObject CreatureEnhanceToggleScript;

	public GameObject CreatureEvoLock;

	public Collider CreatureEvoCol;

	public GameObject CreatureEvoToggleScript;

	public GameObject EvoStackParent;

	public UILabel EvoStackLabel;

	public UILabel TitleLabel;

	public GameObject AwakenTab;

	[HideInInspector]
	public string TabToJumpTo = string.Empty;

	[Header("Loc Strings for UIToggle Titles")]
	public string PowerUpText = "!!CREATURE_POWERUP";

	public string EnhanceText = "!!ENHANCE_CREATURES";

	public string AwakenText = "!!AWAKENING";

	public void Populate()
	{
		Singleton<TutorialController>.Instance.AdvanceIfTargetingBuilding("TBuilding_Lab");
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Lab_Evo"))
		{
			CreatureEvoLock.SetActive(false);
			CreatureEvoCol.enabled = true;
			CreatureEvoToggleScript.SetActive(true);
		}
		else
		{
			CreatureEvoLock.SetActive(true);
			CreatureEvoCol.enabled = false;
			CreatureEvoToggleScript.SetActive(false);
		}
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Lab_Enhance"))
		{
			CreatureEnhanceLock.SetActive(false);
			CreatureEnhanceCol.enabled = true;
			CreatureEnhanceToggleScript.SetActive(true);
		}
		else
		{
			CreatureEnhanceLock.SetActive(true);
			CreatureEnhanceCol.enabled = false;
			CreatureEnhanceToggleScript.SetActive(false);
		}
		UpdateBadgeCount();
		StartCoroutine(JumpToTab());
	}

	private IEnumerator JumpToTab()
	{
		yield return new WaitForEndOfFrame();
		if (TabToJumpTo == "Awaken")
		{
			TabToJumpTo = string.Empty;
			AwakenTab.SendMessage("OnClick");
		}
	}

	public void SetTitleLabel(string inLabelText)
	{
		if (TitleLabel != null)
		{
			TitleLabel.text = KFFLocalization.Get("!!BUILDING_LAB_LONG") + ": " + KFFLocalization.Get(inLabelText);
		}
	}

	public void Hide()
	{
		HideTween.Play();
	}

	public void OnClickCreatureFusion()
	{
		Singleton<XpFusionController>.Instance.Populate();
	}

	public IEnumerator CreatureFusionAfterDelay()
	{
		yield return new WaitForSeconds(0.4f);
		Singleton<XpFusionController>.Instance.Populate();
	}

	public void OnClickCreatureEvo()
	{
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Lab_Evo"))
		{
			Singleton<EvoScreenController>.Instance.Populate();
		}
	}

	public IEnumerator CreatureEvoAfterDelay()
	{
		yield return new WaitForSeconds(0.4f);
		Singleton<EvoScreenController>.Instance.Populate();
	}

	public void OnClickGemCrafting()
	{
	}

	public void OnClickGemFusion()
	{
	}

	public void OnClickEnhanceCreatures()
	{
		if (Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked("Lab_Enhance"))
		{
			Singleton<EnhanceCreatureScreenController>.Instance.Populate();
		}
	}

	public IEnumerator EnhanceCreaturesAfterDelay()
	{
		yield return new WaitForSeconds(0.4f);
		Singleton<EnhanceCreatureScreenController>.Instance.Populate();
	}

	public void OnClosePanel()
	{
		Singleton<TownHudController>.Instance.ReturnToTownView();
	}

	private void Update()
	{
		if (EvoStackParent.transform.parent.gameObject.activeInHierarchy)
		{
			UpdateBadgeCount();
		}
	}

	private void UpdateBadgeCount()
	{
		int num = Singleton<PlayerInfoScript>.Instance.StateData.BadgeCounts[1];
		if (num > 0)
		{
			EvoStackParent.SetActive(true);
			EvoStackLabel.text = num.ToString();
		}
		else
		{
			EvoStackParent.SetActive(false);
		}
	}
}

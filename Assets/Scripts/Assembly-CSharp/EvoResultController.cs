using UnityEngine;

public class EvoResultController : Singleton<EvoResultController>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowStatsPanelTween;

	public UITweenController ShowFrontBlackTween;

	public UITweenController ShowCreatureBackgroundTween;

	public UITweenController HideCreatureBackgroundTween;

	public EvoStatsPanelController StatsPanel;

	public UIGrid InfoRarityStarsGrid;

	public Transform[] InfoRarityStars = new Transform[5];

	public GameObject EggShatterFX;

	public Transform EggShatter2DSpawnPoint;

	public Transform EggShatter3DSpawnPoint;

	public GameObject ResultCreatureBGFX;

	public AnimatedBannerScript BannerAnim;

	public GameObject[] GemSlots = new GameObject[3];

	public UILabel EvoResultCreatureName;

	public CreatureItem EvoResultCreature;

	public Transform ResultCreatureSpawnPoint;

	public void Awake()
	{
	}

	public void PopulateCurrentCreature()
	{
		if (InfoRarityStarsGrid != null)
		{
			for (int i = 0; i < 5; i++)
			{
				InfoRarityStars[i].gameObject.SetActive(i < EvoResultCreature.StarRating);
			}
			if (InfoRarityStarsGrid != null)
			{
				InfoRarityStarsGrid.Reposition();
			}
		}
		StatsPanel.Populate();
	}

	public void OnEvoResultFinished()
	{
		Unload();
		Transform[] infoRarityStars = InfoRarityStars;
		foreach (Transform transform in infoRarityStars)
		{
			transform.gameObject.SetActive(false);
		}
	}

	public void Unload()
	{
	}
}

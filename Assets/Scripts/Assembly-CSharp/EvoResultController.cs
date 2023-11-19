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
	public Transform[] InfoRarityStars;
	public GameObject EggShatterFX;
	public Transform EggShatter2DSpawnPoint;
	public Transform EggShatter3DSpawnPoint;
	public GameObject ResultCreatureBGFX;
	public AnimatedBannerScript BannerAnim;
	public GameObject[] GemSlots;
	public UILabel EvoResultCreatureName;
	public Transform ResultCreatureSpawnPoint;
}

using UnityEngine;
using System.Collections.Generic;

public class GachaOpenSequencer : Singleton<GachaOpenSequencer>
{
	public float CreatureHeightMod;
	public int BattleCameraDepth;
	public UITweenController FadeToBlackTween;
	public UITweenController FadeBackInTween;
	public UITweenController CreatureInfoHideTween;
	public UITweenController CreatureChestOpenTween;
	public UITweenController ShowInfoPanelCloseButtonTween;
	public CreatureStatsPanel StatsPanel;
	public Transform ChestNode;
	public Camera ChestCamera;
	public Camera CreatureCamera;
	public List<Transform> CardNodes;
	public GameObject tipsPanel;
	public GameObject VFX_CardSwap_Prefab;
	public GameObject CreatureNode;
	public GameObject[] RarityToppers;
	public GameObject[] RarityGlows;
	public GameObject[] RarityPlatforms;
}

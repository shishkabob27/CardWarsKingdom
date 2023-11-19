using UnityEngine;
using System.Collections.Generic;

public class BattleIntroController : Singleton<BattleIntroController>
{
	public Animation CharacterP1;
	public Animation CharacterP2;
	public UILabel NameP1;
	public UILabel NameP2;
	public UILabel Arena;
	public AnimationClip[] introCameraClips;
	public UITweenController[] IntroBannerTweens;
	public UITweenController[] OutroBannerTweens;
	public UITweenController StandardEnemyTween;
	public UITweenController BossEnemyTween;
	public int EnemyIndex;
	public List<UILabel> SubtitleLabels;
	public GameObject VictoryVFXPrefab;
	public int NumberOfVictoryVFX;
	public float VictoryVFXInterval;
	public float VictoryVFXIntervalDelayRandomFactor;
	public bool DebugFireworks;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleIntroController : Singleton<BattleIntroController>
{
	public Animation CharacterP1;

	public Animation CharacterP2;

	private bool initialized;

	public UILabel NameP1;

	public UILabel NameP2;

	public UILabel Arena;

	public AnimationClip[] introCameraClips;

	public UITweenController[] IntroBannerTweens;

	public UITweenController[] OutroBannerTweens;

	private bool tweenStartedFlag;

	public UITweenController StandardEnemyTween;

	public UITweenController BossEnemyTween;

	public int EnemyIndex = 2;

	public List<UILabel> SubtitleLabels = new List<UILabel>();

	private float nameBGPadding = 100f;

	private int mCurrentIntroStep;

	public GameObject VictoryVFXPrefab;

	public int NumberOfVictoryVFX = 5;

	public float VictoryVFXInterval = 0.3f;

	public float VictoryVFXIntervalDelayRandomFactor = 0.2f;

	private UITweenController mCurrentTweener;

	public bool DebugFireworks;

	public void LaunchBattleIntro()
	{
		LeaderItem leader = Singleton<DWGame>.Instance.GetLeader(PlayerType.User);
		LeaderItem leader2 = Singleton<DWGame>.Instance.GetLeader(PlayerType.Opponent);
		QuestDataManager instance = QuestDataManager.Instance;
		PlayerInfoScript instance2 = Singleton<PlayerInfoScript>.Instance;
		QuestData currentActiveQuest = instance2.StateData.CurrentActiveQuest;
		if (!instance2.StateData.MultiplayerMode)
		{
			if (currentActiveQuest.IsBossQuest())
			{
				IntroBannerTweens[EnemyIndex] = BossEnemyTween;
			}
			else
			{
				IntroBannerTweens[EnemyIndex] = StandardEnemyTween;
			}
		}
		if (NameP1 != null && leader != null)
		{
			if (instance2.StateData.MultiplayerMode)
			{
				NameP1.text = Singleton<PlayerInfoScript>.Instance.SaveData.MultiplayerPlayerName;
			}
			else
			{
				NameP1.text = leader.Form.Name.ToUpper();
			}
		}
		if (NameP2 != null && leader2 != null)
		{
			if (instance2.StateData.MultiplayerMode)
			{
				NameP2.text = instance2.PvPData.OpponentName;
			}
			else
			{
				NameP2.text = leader2.Form.Name.ToUpper();
			}
		}
		if (Arena != null && currentActiveQuest != null)
		{
			Arena.text = currentActiveQuest.LevelName;
		}
		if (Singleton<DWGameCamera>.Instance.MainCamLookAt != null)
		{
			Singleton<DWGameCamera>.Instance.MainCamLookAt.mFollowFlag = false;
		}
		for (int i = 0; i < 2; i++)
		{
			CharAnimType animType = ((i != 0) ? CharAnimType.P2Intro : CharAnimType.P1Intro);
			Singleton<CharacterAnimController>.Instance.PlayHeroAnim(i, animType);
		}
		StartCoroutine("TriggerIntroCameraCuts");
	}

	private IEnumerator TriggerIntroCameraCuts()
	{
		bool inIntroBattle = Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle");
		GameObject BattleCam = Singleton<DWGameCamera>.Instance.MainCam.gameObject;
		Animation anim = BattleCam.GetComponent<Animation>();
		for (mCurrentIntroStep = 0; mCurrentIntroStep < introCameraClips.Length; mCurrentIntroStep++)
		{
			if (mCurrentIntroStep == 1)
			{
				Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.User), VOEvent.P1Intro);
			}
			else if (mCurrentIntroStep == 2)
			{
				Singleton<SLOTAudioManager>.Instance.TriggerVOEvent(Singleton<DWGame>.Instance.GetCharacter(PlayerType.Opponent), VOEvent.P2Intro);
			}
			if (!inIntroBattle || mCurrentIntroStep > 2)
			{
				(mCurrentTweener = IntroBannerTweens[mCurrentIntroStep]).Play();
			}
			AnimationClip clip = introCameraClips[mCurrentIntroStep];
			anim.Play(clip.name);
			while (anim.isPlaying)
			{
				yield return 0;
			}
		}
		yield return null;
		FinishIntro();
	}

	public void PlayWinnerCamera(PlayerType player)
	{
		StartCoroutine(TriggerWinnerCamera(player));
		StartCoroutine(TriggerHeroWinAnim(player));
		StartCoroutine(TriggerVictoryBannerAnim(true));
	}

	public void PlayLoserBanner()
	{
		StartCoroutine(TriggerVictoryBannerAnim(false));
	}

	private IEnumerator TriggerVictoryBannerAnim(bool win)
	{
		int i = ((!win) ? 1 : 0);
		yield return new WaitForSeconds(0.5f);
		if (win)
		{
			Singleton<BattleResultsController>.Instance.ShowCoolGuyBannerTween.Play();
		}
		else
		{
			Singleton<BattleResultsController>.Instance.ShowDweebBannerTween.Play();
		}
		yield return new WaitForSeconds(2.333f);
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			if (win)
			{
				Singleton<BattleResultsController>.Instance.VictoryBannerBGTween.Play();
				StartCoroutine(TriggerVictoryVFX());
			}
			else
			{
				Singleton<BattleResultsController>.Instance.FailBannerBGTween.Play();
			}
			Singleton<BattleResultsController>.Instance.WinLoseBanner[i].SetActive(true);
		}
		yield return null;
	}

	private IEnumerator TriggerVictoryTextAnim()
	{
		Singleton<BattleResultsController>.Instance.PlayBannerAnim();
		yield return null;
	}

	private IEnumerator TriggerVictoryVFX()
	{
		for (int i = 0; i < NumberOfVictoryVFX; i++)
		{
			float delay = Random.Range(0f - VictoryVFXIntervalDelayRandomFactor, VictoryVFXIntervalDelayRandomFactor);
			float posx = Random.Range(0f, 1f);
			float posy = Random.Range(0f, 1f);
			Vector2 screenPos = new Vector2(posx, posy);
			Vector3 spawnPos = Singleton<DWGameCamera>.Instance.BattleUICam.ViewportToWorldPoint(screenPos);
			if (VictoryVFXPrefab != null)
			{
				SLOTGame.InstantiateFX(VictoryVFXPrefab, spawnPos, Quaternion.Euler(Vector3.zero));
			}
			yield return new WaitForSeconds(VictoryVFXInterval + delay);
		}
	}

	private IEnumerator TriggerWinnerCamera(PlayerType player)
	{
		yield return new WaitForSeconds(0.3f);
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/UI_PlayerWin");
		LeaderItem winner = Singleton<DWGame>.Instance.GetLeader(player);
		GameObject BattleCam = Singleton<DWGameCamera>.Instance.MainCam.gameObject;
		Singleton<DWGameCamera>.Instance.MainCamLookAt.mFollowFlag = false;
		Animation anim = BattleCam.GetComponent<Animation>();
		string clipName = winner.SelectedSkin.CameraWinAnim;
		anim.Play(clipName);
		while (anim.isPlaying)
		{
			yield return 0;
		}
		OnFinishWinCamera(player);
	}

	private IEnumerator TriggerHeroWinAnim(PlayerType player)
	{
		yield return new WaitForSeconds(0.3f);
		Singleton<CharacterAnimController>.Instance.PlayHeroAnim(player, CharAnimType.Win);
	}

	private void OnTapToSkipIntro()
	{
		if (!Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle") && mCurrentIntroStep != 0)
		{
			if (mCurrentTweener != null)
			{
				mCurrentTweener.End();
			}
			StopCoroutine("TriggerIntroCameraCuts");
			Animation component = Singleton<DWGameCamera>.Instance.MainCam.GetComponent<Animation>();
			component.Stop();
			FinishIntro();
			Singleton<CharacterAnimController>.Instance.ForceIdle(PlayerType.User);
			Singleton<CharacterAnimController>.Instance.ForceIdle(PlayerType.Opponent);
		}
	}

	public void OnFinishWinCamera(PlayerType player)
	{
		if (!Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode && player == PlayerType.User)
		{
			Singleton<BattleResultsController>.Instance.ShowVictory();
		}
	}

	public void FinishIntro()
	{
		Singleton<CharacterAnimController>.Instance.ResetCharacterAnimState();
		Singleton<CharacterAnimController>.Instance.StopCharacterFidgets(false);
		Singleton<DWGameCamera>.Instance.InitPIPCams();
		Singleton<BattleHudController>.Instance.UpdateTopBar();
		Singleton<DWGameCamera>.Instance.MainCamLookAt.mFollowFlag = true;
		if (Singleton<TutorialController>.Instance.IsBlockActive("IntroBattle"))
		{
			Singleton<DWGame>.Instance.SkipCoinFlipForIntroBattle();
		}
		else
		{
			Singleton<DWGame>.Instance.SetGameState(GameState.FirstTurnCoinFlip);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Singleton<DWGame>.Instance.GetCurrentGameState() == GameState.WaitForIntro)
		{
			OnTapToSkipIntro();
		}
		if (DebugFireworks)
		{
			StartCoroutine(TriggerVictoryVFX());
			DebugFireworks = false;
		}
	}
}

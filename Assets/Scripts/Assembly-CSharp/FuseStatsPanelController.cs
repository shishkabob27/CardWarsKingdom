using System.Collections;
using UnityEngine;

public class FuseStatsPanelController : Singleton<FuseStatsPanelController>
{
	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowAfterTween;

	public UITweenController HideAfterTween;

	public UITweenController ShowAfterPassiveTween;

	public UILabel CreatureName;

	public UISprite[] RarityStars;

	public UISprite FactionIcon;

	public UISprite ExpBarFill;

	public UILabel ExpPct;

	public UILabel ExpInLevel;

	public UILabel ExpReceived;

	public UILabel CreatureLevel;

	public UILabel CreatureLevelBefore;

	public UILabel[] GemNames;

	public UILabel[] GemDescriptions;

	public UILabel DEF;

	public UILabel DEX;

	public UILabel HP;

	public UILabel INT;

	public UILabel RES;

	public UILabel STR;

	public UILabel PassiveSkill;

	public UILabel PassiveLevel;

	public UILabel PassiveSkillScale;

	public UILabel PassiveLevelScale;

	public UILabel DEFBefore;

	public UILabel DEXBefore;

	public UILabel HPBefore;

	public UILabel INTBefore;

	public UILabel RESBefore;

	public UILabel STRBefore;

	public UILabel PassiveSkillBefore;

	public UILabel PassiveLevelBefore;

	public GameObject PassiveLevelUpArrow;

	public CreatureItem ResultCreature;

	public CreatureItem ResultCreatureBefore;

	private bool mDone;

	private XPLevelData mCurrentLevelData;

	private bool mExpLoopSoundStarted;

	private float mTickTimer;

	private int mExpPerSecond;

	public int EarnedXP;

	public float ExpTickTime = 2f;

	private int mCurrentExp;

	private int mGivingExp;

	private bool mStopAnimate = true;

	private bool mSkip;

	private bool mShowingLevelUp;

	private void Init()
	{
		mDone = false;
		mSkip = false;
		mCurrentExp = ResultCreatureBefore.Xp;
		mGivingExp = EarnedXP;
		mExpPerSecond = (int)((float)EarnedXP / ExpTickTime);
	}

	public void ShowStats()
	{
		if (Singleton<TutorialController>.Instance.IsBlockActive("XpFusion"))
		{
			UICamera.LockInput();
		}
		Init();
		ShowTween.PlayWithCallback(TriggerExpTick);
		PopulateStats();
	}

	private void TriggerExpTick()
	{
		mCurrentLevelData = ResultCreatureBefore.GetLevelData();
		mStopAnimate = false;
	}

	public void HideStats()
	{
		HideTween.Play();
		ResultCreature = null;
		ResultCreatureBefore = null;
	}

	public void PopulateStats()
	{
		ResultCreature.Form.ParseKeywords();
		XPLevelData levelData = ResultCreatureBefore.GetLevelData();
		levelData.PopulateUI(true, null, ExpInLevel, ExpPct, ExpBarFill);
		UILabel expInLevel = ExpInLevel;
		expInLevel.text = expInLevel.text + " " + KFFLocalization.Get("!!EXP");
		CreatureLevelBefore.text = levelData.mCurrentLevel.ToString();
		if ((bool)CreatureName)
		{
			CreatureName.text = ResultCreature.Form.Name;
		}
		if ((bool)FactionIcon)
		{
			FactionIcon.spriteName = ResultCreature.Form.Faction.IconTexture();
		}
		for (int i = 0; i < 5; i++)
		{
			RarityStars[i].gameObject.SetActive(i < ResultCreature.StarRating);
		}
		if ((bool)PassiveLevel)
		{
			if (ResultCreature.PassiveLevelString() != ResultCreatureBefore.PassiveLevelString())
			{
				PassiveLevel.text = ResultCreature.PassiveLevelString();
				if ((bool)PassiveLevelScale)
				{
					PassiveLevelScale.text = ResultCreature.PassiveLevelString();
				}
				if ((bool)PassiveSkill)
				{
					PassiveSkill.text = ResultCreature.BuildPassiveDescriptionString(false);
				}
				if ((bool)PassiveSkillScale)
				{
					PassiveSkillScale.text = ResultCreature.BuildPassiveDescriptionString(false);
				}
			}
			else
			{
				PassiveLevel.text = string.Empty;
				if ((bool)PassiveSkill)
				{
					PassiveSkill.text = string.Empty;
				}
			}
		}
		if ((bool)DEF)
		{
			DEF.text = ResultCreature.DEF + "%";
		}
		if ((bool)DEX)
		{
			DEX.text = ResultCreature.DEX + "%";
		}
		if ((bool)HP)
		{
			HP.text = ResultCreature.HP.ToString();
		}
		if ((bool)INT)
		{
			INT.text = ResultCreature.INT.ToString();
		}
		if ((bool)RES)
		{
			RES.text = ResultCreature.RES + "%";
		}
		if ((bool)STR)
		{
			STR.text = ResultCreature.STR.ToString();
		}
		XPLevelData levelData2 = ResultCreature.GetLevelData();
		CreatureLevel.text = ((!levelData2.mIsAtMaxLevel) ? levelData2.mCurrentLevel.ToString() : (levelData2.mCurrentLevel + " MAX"));
		PassiveLevelBefore.text = ResultCreatureBefore.PassiveLevelString();
		PassiveSkillBefore.text = ResultCreatureBefore.BuildPassiveDescriptionString(false);
		DEFBefore.text = ResultCreatureBefore.DEF + "%";
		DEXBefore.text = ResultCreatureBefore.DEX + "%";
		HPBefore.text = ResultCreatureBefore.HP.ToString();
		INTBefore.text = ResultCreatureBefore.INT.ToString();
		RESBefore.text = ResultCreatureBefore.RES + "%";
		STRBefore.text = ResultCreatureBefore.STR.ToString();
		DEF.color = ((ResultCreature.DEF <= ResultCreatureBefore.DEF) ? DEFBefore.color : Color.green);
		DEX.color = ((ResultCreature.DEX <= ResultCreatureBefore.DEX) ? DEXBefore.color : Color.green);
		HP.color = ((ResultCreature.HP <= ResultCreatureBefore.HP) ? HPBefore.color : Color.green);
		INT.color = ((ResultCreature.INT <= ResultCreatureBefore.INT) ? INTBefore.color : Color.green);
		RES.color = ((ResultCreature.RES <= ResultCreatureBefore.RES) ? RESBefore.color : Color.green);
		STR.color = ((ResultCreature.STR <= ResultCreatureBefore.STR) ? STRBefore.color : Color.green);
	}

	private void PopulateCardsInStats()
	{
	}

	private void Update()
	{
		if (mStopAnimate)
		{
			return;
		}
		mCurrentLevelData = ResultCreatureBefore.GetLevelDataAt(mCurrentExp);
		mTickTimer += Time.deltaTime;
		float num = 1f / (float)mExpPerSecond;
		int num2 = ((!mSkip) ? ((int)(mTickTimer / num)) : mGivingExp);
		if (num2 >= 1 || mGivingExp <= 0)
		{
			mTickTimer -= (float)num2 * num;
			if (num2 > mGivingExp)
			{
				num2 = mGivingExp;
			}
			if (!mSkip && num2 > mCurrentLevelData.mXPToPassCurrentLevel)
			{
				num2 = mCurrentLevelData.mXPToPassCurrentLevel;
			}
			mGivingExp -= num2;
			mCurrentExp += num2;
			if (mCurrentExp >= mCurrentLevelData.mXPToPassCurrentLevel)
			{
				PlaySound(false);
				StartCoroutine(ShowLevelUp());
			}
			else if (mGivingExp <= 0)
			{
				PlaySound(false);
				StopBarAnimation();
			}
			else if (!mCurrentLevelData.mIsAtMaxLevel)
			{
				PlaySound(true);
			}
			UpdateRewardValues();
		}
	}

	private void UpdateRewardValues()
	{
		ExpReceived.text = ((mGivingExp <= 0) ? string.Empty : (mGivingExp + " " + KFFLocalization.Get("!!EXP")));
		int num = ((!mShowingLevelUp) ? mCurrentLevelData.mXPEarnedWithinCurrentLevel : mCurrentLevelData.mTotalXPInCurrentLevel);
		float num2 = ((!mShowingLevelUp) ? mCurrentLevelData.mPercentThroughCurrentLevel : 1f);
		ExpInLevel.text = num + " / " + mCurrentLevelData.mTotalXPInCurrentLevel + " " + KFFLocalization.Get("!!EXP");
		ExpBarFill.fillAmount = num2;
		ExpPct.text = (int)(num2 * 100f) + "%";
	}

	private IEnumerator ShowLevelUp()
	{
		mStopAnimate = true;
		mShowingLevelUp = true;
		Singleton<SLOTAudioManager>.Instance.PlaySound("ui/UI_LevelUp_Panel");
		yield return new WaitForSeconds(0.5f);
		mShowingLevelUp = false;
		mStopAnimate = false;
	}

	private void StopBarAnimation()
	{
		mStopAnimate = true;
		ShowAfterTween.PlayWithCallback(PlayEmphasizePassiveLevelUp);
		PassiveLevelUpArrow.SetActive(ResultCreature.PassiveLevelString() != ResultCreatureBefore.PassiveLevelString());
	}

	private void PlaySound(bool enabled)
	{
		if (enabled && !mExpLoopSoundStarted)
		{
			Singleton<SLOTAudioManager>.Instance.PlaySound("ui/UI_ExpLoop");
			mExpLoopSoundStarted = true;
		}
		else if (!enabled && mExpLoopSoundStarted)
		{
			Singleton<SLOTAudioManager>.Instance.StopSound("ui/UI_ExpLoop");
			mExpLoopSoundStarted = false;
		}
	}

	private void PlayEmphasizePassiveLevelUp()
	{
		if (ResultCreature.PassiveLevelString() != ResultCreatureBefore.PassiveLevelString())
		{
			ShowAfterPassiveTween.Play();
		}
		if (Singleton<TutorialController>.Instance.IsBlockActive("XpFusion"))
		{
			UICamera.UnlockInput();
		}
		mDone = true;
	}

	public void OnTapScreen()
	{
		if (mDone)
		{
			HideTween.Play();
			Singleton<XpFusionController>.Instance.OnClickCloseResult();
		}
		else
		{
			mSkip = true;
		}
	}
}

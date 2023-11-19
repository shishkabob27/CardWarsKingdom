using System.Collections;
using UnityEngine;

public class HeroPortraitScript : MonoBehaviour
{
	public UITweenController PowerUpBarCompleteTween;

	public int player;

	public UITexture HeroPortrait;

	public UILabel HeroName;

	public UISprite HeroHPBarDamage;

	public UISprite HeroHPBar;

	public UILabel HeroHP;

	public UILabel AbilityName;

	public UILabel AbilityDesc;

	public UILabel AbilityCost;

	public float HealthBarLerpSpeed = 1f;

	public float DamageBarDelay = 2f;

	public float DamageBarSpeed = 0.7f;

	public UISprite MainBar;

	public UISprite DamageBar;

	public UILabel DamageValue;

	public UISprite PowerUpBar;

	public GameObject PowerUpBarParent;

	private PlayerState mPlayer;

	private float mTargetHealth = 1f;

	private float mDamageBarTimer = 1f;

	private float mTargetPowerUpLevel;

	private float mDisplayedPowerUpLevel;

	private bool mPowerUpTickEffectDone;

	private bool mPlayingPowerUpMeterTween;

	public void Init(PlayerState playerState)
	{
		mPlayer = playerState;
		HeroName.text = mPlayer.Leader.Form.Name;
		HeroHP.text = string.Empty;
		HeroHPBar.fillAmount = 1f;
		HeroHPBarDamage.fillAmount = 1f;
		ResetPowerUpMeter();
	}

	private void Update()
	{
		if (mPlayer == null)
		{
			return;
		}
		HeroHPBar.fillAmount = Mathf.Lerp(HeroHPBar.fillAmount, mTargetHealth, Time.deltaTime * HealthBarLerpSpeed);
		if (mDamageBarTimer == 2f)
		{
		}
		if (mDamageBarTimer == -1f)
		{
			HeroHPBarDamage.fillAmount -= Time.deltaTime * DamageBarSpeed;
			if (HeroHPBarDamage.fillAmount < mTargetHealth)
			{
				HeroHPBarDamage.fillAmount = mTargetHealth;
			}
		}
		else
		{
			mDamageBarTimer -= Time.deltaTime;
			if (mDamageBarTimer <= 0f)
			{
				mDamageBarTimer = -1f;
			}
		}
		if (mPlayingPowerUpMeterTween)
		{
		}
	}

	private IEnumerator PlayPowerUpMeterEffect()
	{
		Vector2 screenPos = Singleton<DWGameCamera>.Instance.Battle3DUICam.WorldToScreenPoint(Singleton<BattleHudController>.Instance.CardDroppedPos);
		Vector3 startPos = Singleton<DWGameCamera>.Instance.BattleUICam.ScreenToWorldPoint(screenPos);
		startPos.z = 0f;
		GameObject flyObj = SLOTGame.InstantiateFX(Singleton<BattleHudController>.Instance.PowerUpVFXFly) as GameObject;
		flyObj.transform.position = startPos;
		Vector3 endPosLocal = PowerUpBar.transform.localPosition;
		endPosLocal.x += (float)PowerUpBar.width * PowerUpBar.fillAmount;
		Vector3 endPos = PowerUpBar.transform.parent.TransformPoint(endPosLocal);
		int i = 3;
		Vector3[] lootFlyPath = new Vector3[i];
		lootFlyPath[0] = startPos;
		lootFlyPath[1] = Vector3.Lerp(startPos, endPos, 0.5f);
		lootFlyPath[1].y = lootFlyPath[0].y;
		lootFlyPath[2] = endPos;
		iTween.MoveTo(flyObj, iTween.Hash("path", lootFlyPath, "time", Singleton<BattleHudController>.Instance.PowerUpVFXFlyTime, "easetype", iTween.EaseType.linear));
		yield return new WaitForSeconds(Singleton<BattleHudController>.Instance.PowerUpVFXFlyTime);
		Object.Destroy(flyObj);
		GameObject addFx = PowerUpBar.transform.InstantiateAsChild(Singleton<BattleHudController>.Instance.PowerUpVFXAdd);
		addFx.transform.position = endPos;
		while (mDisplayedPowerUpLevel != mTargetPowerUpLevel)
		{
			mDisplayedPowerUpLevel = mDisplayedPowerUpLevel.TickTowards(mTargetPowerUpLevel, Singleton<BattleHudController>.Instance.ActionPointTickDownSpeed);
			PowerUpBar.fillAmount = mDisplayedPowerUpLevel / (float)mPlayer.Leader.Form.APThreshold;
			yield return null;
		}
	}

	private Vector3 ScreenPosToCardPos(Vector3 screenPos)
	{
		screenPos.z = Singleton<HandCardController>.Instance.ScreenZAdjust;
		Vector3 result = Singleton<DWGameCamera>.Instance.Battle3DUICam.ScreenToWorldPoint(screenPos);
		result.z = Singleton<HandCardController>.Instance.CardZDepth;
		return result;
	}

	public bool SetHealth(int currentHP, int maxHP, bool playDamageAnimation)
	{
		float num = (float)currentHP / (float)maxHP;
		bool flag = num < mTargetHealth;
		mTargetHealth = num;
		if (playDamageAnimation)
		{
			mDamageBarTimer = DamageBarDelay;
		}
		else
		{
			HeroHPBarDamage.fillAmount = mTargetHealth;
			HeroHPBar.fillAmount = mTargetHealth;
			mDamageBarTimer = -1f;
		}
		HeroHP.text = currentHP.ToString();
		if (flag)
		{
			ShakePortraitOnDamage();
		}
		return flag;
	}

	private void ShakePortraitOnDamage()
	{
		iTween.ShakePosition(base.gameObject, iTween.Hash("amount", Vector3.one * 0.2f, "time", 0.5f));
	}

	public void ShowLeaderAbility()
	{
	}

	public void HideLeaderAbility()
	{
		if (mPlayer.Type == PlayerType.User)
		{
			AbilityDesc.text = string.Empty;
		}
	}

	public void ActivateLeaderAbility()
	{
	}

	public void ResetPowerUpMeter()
	{
		mTargetPowerUpLevel = 0f;
		mDisplayedPowerUpLevel = 0f;
		PowerUpBar.fillAmount = 0f;
	}

	public void FillPowerUpMeter(float amount)
	{
		mTargetPowerUpLevel += amount;
		StartCoroutine(PlayPowerUpMeterEffect());
	}

	public IEnumerator OnPowerUpMeterFull()
	{
		mTargetPowerUpLevel = mPlayer.Leader.Form.APThreshold;
		while (mDisplayedPowerUpLevel < mTargetPowerUpLevel)
		{
			yield return null;
		}
		mPlayingPowerUpMeterTween = true;
		PowerUpBarCompleteTween.PlayWithCallback(PowerUpMeterTweenFinsihed);
		while (mPlayingPowerUpMeterTween)
		{
			yield return null;
		}
	}

	private void PowerUpMeterTweenFinsihed()
	{
		mPlayingPowerUpMeterTween = false;
		ResetPowerUpMeter();
	}
}

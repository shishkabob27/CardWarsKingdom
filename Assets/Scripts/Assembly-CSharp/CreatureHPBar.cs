using UnityEngine;

public class CreatureHPBar : MonoBehaviour
{
	public float HealthBarLerpSpeed = 1f;

	public float DamageBarDelay = 2f;

	public float DamageBarSpeed = 0.7f;

	public float FillOffset;

	public Color StrengthColor;

	public Color MagicColor;

	public Color HybridColor;

	public UITweenController FlashDamagePredictionTween;

	public UITweenController FlashDamagePredictionOnceTween;

	public UITweenController ChangeAttackColorTween;

	public UITweenController ScaleAttackLabelTween;

	public UITweenController FlashAttackCostTween;

	public UITweenController FlashAttackValueTween;

	public UITweenController FlashMagicValueTween;

	public UISprite MainBar;

	public UISprite DamageBar;

	public UISprite DamagePredictionBar;

	public UILabel HPValue;

	public UILabel STRValue;

	public UILabel MGCValue;

	public UISprite STRValueTrim;

	public UILabel AttackCost;

	public GameObject CanAttackIndicator;

	public GameObject TargetableIndicator;

	private int mTargetHealth;

	private float mDamageBarTimer = 1f;

	public CreatureState mCreature;

	private GameObject mCreatureObject;

	private float mPredictedDamage;

	public void SetPredictedDamage(float damage)
	{
		mPredictedDamage = damage;
		if (damage > 0f)
		{
			FlashDamagePredictionTween.Play();
			return;
		}
		FlashDamagePredictionTween.StopAndReset();
		FlashDamagePredictionOnceTween.StopAndReset();
	}

	public CreatureHPBar FlashPredictedDamageOnce(float damage)
	{
		mPredictedDamage = damage;
		FlashDamagePredictionTween.StopAndReset();
		FlashDamagePredictionOnceTween.Play();
		return this;
	}

	public void Init(CreatureState creature)
	{
		mCreature = creature;
		mCreatureObject = Singleton<DWBattleLane>.Instance.GetCreatureObject(creature);
		mTargetHealth = mCreature.HP;
		float offsetFillAmount = GetOffsetFillAmount((float)mTargetHealth / (float)mCreature.MaxHP);
		DamageBar.fillAmount = offsetFillAmount;
		MainBar.fillAmount = offsetFillAmount;
		DamagePredictionBar.fillAmount = offsetFillAmount;
		mDamageBarTimer = -1f;
		UpdateRotation();
		SetValuesOnLabels();
		SetAttackValues(DamageType.Physical);
	}

	private float GetOffsetFillAmount(float healthPercent)
	{
		if (healthPercent == 1f)
		{
			return 1f;
		}
		if (healthPercent <= 0f)
		{
			return 0f;
		}
		return (healthPercent - 0.5f) * (1f - FillOffset * 2f) + 0.5f;
	}

	private void Update()
	{
		if (mCreatureObject == null)
		{
			Singleton<BattleHudController>.Instance.HPBars.Remove(this);
			Object.Destroy(base.gameObject);
			return;
		}
		float offsetFillAmount = GetOffsetFillAmount((float)mTargetHealth / (float)mCreature.MaxHP);
		float num = ((mPredictedDamage >= (float)mCreature.HP) ? offsetFillAmount : ((mCreature.HP != mCreature.MaxHP) ? ((1f - FillOffset * 2f) * mPredictedDamage / (float)mCreature.MaxHP) : GetOffsetFillAmount(mPredictedDamage / (float)mCreature.MaxHP)));
		DamagePredictionBar.fillAmount = Mathf.Lerp(DamagePredictionBar.fillAmount, offsetFillAmount, Time.deltaTime * HealthBarLerpSpeed);
		MainBar.fillAmount = DamagePredictionBar.fillAmount - num;
		if (mDamageBarTimer == 2f)
		{
		}
		if (mDamageBarTimer == -1f)
		{
			DamageBar.fillAmount -= Time.deltaTime * DamageBarSpeed;
			if (DamageBar.fillAmount < offsetFillAmount)
			{
				DamageBar.fillAmount = offsetFillAmount;
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
	}

	private void LateUpdate()
	{
		UpdateRotation();
	}

	private void UpdateRotation()
	{
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
	}

	public void ShowDamage(int amount)
	{
		mTargetHealth -= amount;
		mDamageBarTimer = DamageBarDelay;
		SetValuesOnLabels();
	}

	public void ShowHealing(int amount)
	{
		mTargetHealth += amount;
		SetValuesOnLabels();
	}

	public void SetHealth(int amount)
	{
		mTargetHealth = amount;
		SetValuesOnLabels();
	}

	public void SetValuesOnLabels()
	{
		HPValue.text = mTargetHealth.ToString();
	}

	public void SetAttackValues(DamageType attackType)
	{
		STRValue.text = ((int)mCreature.STR).ToString();
		MGCValue.text = ((int)mCreature.INT).ToString();
		AttackCost.text = mCreature.AttackCost.ToString();
	}

	public void StaminaChange(int amount)
	{
	}

	public void SetStamina(int amount)
	{
	}

	public void MaxStaminaChange(int amount)
	{
	}

	private void StaminaRotateDone()
	{
	}
}

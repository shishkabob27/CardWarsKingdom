using UnityEngine;

public class CreatureHPBar : MonoBehaviour
{
	public float HealthBarLerpSpeed;
	public float DamageBarDelay;
	public float DamageBarSpeed;
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
}

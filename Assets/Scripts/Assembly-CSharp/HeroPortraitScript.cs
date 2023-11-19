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
	public float HealthBarLerpSpeed;
	public float DamageBarDelay;
	public float DamageBarSpeed;
	public UISprite MainBar;
	public UISprite DamageBar;
	public UILabel DamageValue;
	public UISprite PowerUpBar;
	public GameObject PowerUpBarParent;
}

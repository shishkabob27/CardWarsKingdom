using UnityEngine;

public class ShowDamagePopup : MonoBehaviour
{
	public UITweenController DamageTween;

	public UITweenController CritDamageTween;

	public UILabel DamageText;

	public UILabel CritLabel;

	public Color MagicDamageColor = Color.blue;

	public Color HybridDamageColor = Color.magenta;

	public Color HealColor = Color.green;

	private Vector3 mWorldPos;

	public void Init(int amount, Vector3 worldPos, DamageType type, bool isCrit = false)
	{
		mWorldPos = worldPos;
		DamageText.text = amount.ToString();
		if (isCrit)
		{
			CritDamageTween.Play();
			DamageText.color = CritLabel.color;
			DamageText.effectDistance = new Vector2(1f, 1f);
		}
		else
		{
			DamageTween.Play();
			CritLabel.gameObject.SetActive(false);
			switch (type)
			{
			case DamageType.Physical:
				DamageText.color = Color.white;
				DamageText.effectDistance = new Vector2(2f, 2f);
				break;
			case DamageType.Magic:
				DamageText.color = Color.white;
				DamageText.effectColor = MagicDamageColor;
				DamageText.effectDistance = new Vector2(3f, 3f);
				break;
			case DamageType.Hybrid:
				DamageText.color = Color.white;
				DamageText.effectColor = HybridDamageColor;
				DamageText.effectDistance = new Vector2(3f, 3f);
				break;
			case DamageType.Healing:
				DamageText.color = HealColor;
				DamageText.effectColor = Color.black;
				DamageText.effectDistance = new Vector2(2f, 2f);
				break;
			}
		}
		Update();
	}

	private void Update()
	{
		if (Camera.main != null)
		{
			Vector3 position = Camera.main.WorldToScreenPoint(mWorldPos);
			base.transform.position = Singleton<DWGameCamera>.Instance.BattleUICam.ScreenToWorldPoint(position);
			base.transform.SetLocalPositionZ(0f);
		}
	}

	public void OnFinish()
	{
		Object.Destroy(base.gameObject);
	}
}

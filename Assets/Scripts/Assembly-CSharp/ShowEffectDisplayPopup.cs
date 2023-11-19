using UnityEngine;

public class ShowEffectDisplayPopup : MonoBehaviour
{
	public float DisplayTime;

	public UITweenController ShowTween;

	public UITweenController PushUpTween;

	public UILabel DisplayText;

	public UILabel DisplayTextScale;

	public int FontSize = 20;

	public Color DisplayColor = Color.green;

	private Vector3 mWorldPos;

	private DWBattleLaneObject mLaneObj;

	private float mTimer = -1f;

	public void Init(DWBattleLaneObject laneObj, string str, Vector3 worldPos, Color color, Color outlineColor, int fontSize = 20)
	{
		if (ShowTween != null)
		{
			ShowTween.Play();
			mTimer = DisplayTime;
		}
		AttachToLaneObj(laneObj);
		DisplayText.text = str;
		mWorldPos = worldPos;
		DisplayText.color = color;
		DisplayText.effectColor = outlineColor;
		DisplayText.fontSize = fontSize;
		if (DisplayTextScale != null)
		{
			DisplayTextScale.text = str;
			DisplayText.color = Color.Lerp(color, Color.black, 0.5f);
			DisplayText.fontSize = fontSize;
		}
	}

	private void AttachToLaneObj(DWBattleLaneObject laneObj)
	{
		if (laneObj == null)
		{
			return;
		}
		mLaneObj = laneObj;
		if (mLaneObj.AttachedEffectPopup != null)
		{
			mLaneObj.AttachedEffectPopup.mTimer = -1f;
			if (!mLaneObj.AttachedEffectPopup.PushUpTween.AnyTweenPlaying())
			{
				mLaneObj.AttachedEffectPopup.PushUpTween.Play();
			}
		}
		mLaneObj.AttachedEffectPopup = this;
	}

	private void Update()
	{
		if (Camera.main != null)
		{
			Vector3 position = Camera.main.WorldToScreenPoint(mWorldPos);
			base.transform.position = Singleton<DWGameCamera>.Instance.BattleUICam.ScreenToWorldPoint(position);
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
		}
		if (!DisplayText.gameObject.activeSelf)
		{
			DisplayText.gameObject.SetActive(true);
		}
		if (mTimer > 0f)
		{
			mTimer -= Time.deltaTime;
			if (mTimer <= 0f)
			{
				mTimer = -1f;
				PushUpTween.Play();
			}
		}
	}

	public void OnFinish()
	{
		if (mLaneObj != null && mLaneObj.AttachedEffectPopup == this)
		{
			mLaneObj.AttachedEffectPopup = null;
		}
		Object.Destroy(base.gameObject);
	}
}

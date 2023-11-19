using System.Collections;
using UnityEngine;

public class StatusIconItem : MonoBehaviour
{
	public float TickUpTime;

	public UITweenController ShowTween;

	public UITweenController HideTween;

	public UITweenController ShowStackTween;

	public UITweenController ShowEnemyStackTween;

	public UITweenController HideStackTween;

	public UISprite Icon;

	public UILabel Value;

	private float mCurrentAmount;

	private float mTargetAmount;

	private DisplayStackType mDisplayType;

	private int mBaseDepth;

	private bool mOnEnemy;

	public StatusData Status { get; set; }

	public void StartHideTween()
	{
		HideTween.Play();
	}

	public void OnHideTweenComplete()
	{
		NGUITools.Destroy(base.gameObject);
	}

	public void Populate(StatusData status, CreatureState creature)
	{
		Populate(status, status.GetValueString(creature, false));
	}

	public void Populate(StatusData status, GameMessage message)
	{
		mOnEnemy = message.Creature.Owner.Type == PlayerType.Opponent;
		mCurrentAmount = (mTargetAmount = status.GetValueAmount(message));
		mDisplayType = status.DisplayType;
		Populate(status, mDisplayType.Format(mCurrentAmount));
	}

	private void Populate(StatusData status, string valueString)
	{
		Status = status;
		Icon.spriteName = status.StatusIconSprite;
		mBaseDepth = Icon.depth;
		Value.text = valueString;
		if (ShowTween != null)
		{
			ShowTween.Play();
		}
	}

	public IEnumerator UpdateValue(GameMessage message)
	{
		mTargetAmount = Status.GetValueAmount(message);
		Icon.depth = mBaseDepth + 3;
		Value.depth = mBaseDepth + 3;
		if (Value.LabelShadow != null)
		{
			Value.LabelShadow.RefreshShadowLabel();
		}
		if (mOnEnemy)
		{
			ShowEnemyStackTween.Play();
		}
		else
		{
			ShowStackTween.Play();
		}
		float amountPerSecond = (mTargetAmount - mCurrentAmount) / TickUpTime;
		while (true)
		{
			mCurrentAmount = mCurrentAmount.TickTowards(mTargetAmount, amountPerSecond);
			Value.text = mDisplayType.Format(mCurrentAmount);
			if (mCurrentAmount == mTargetAmount)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.1f);
		HideStackTween.GetComponent<TweenPosition>().from = Icon.transform.parent.localPosition;
		HideStackTween.Play();
		Icon.depth = mBaseDepth;
		Value.depth = mBaseDepth;
		if (Value.LabelShadow != null)
		{
			Value.LabelShadow.RefreshShadowLabel();
		}
	}
}

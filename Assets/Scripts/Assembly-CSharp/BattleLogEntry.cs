using UnityEngine;

public class BattleLogEntry : MonoBehaviour
{
	public UITweenController DisappearTween;

	public UITexture Image;

	public GameObject DragAttackIcon;

	private CardData mCard;

	private CreatureItem mCreature;

	private CreatureItem mCreatureTarget;

	public void PopulateCardPlay(CardData card, CreatureState target)
	{
		Singleton<SLOTResourceManager>.Instance.QueueUITextureLoad(card.UITexture, card.AssetBundle, "UI/UI/LoadingPlaceholder", Image);
		DragAttackIcon.SetActive(false);
		mCard = card;
		if (target != null)
		{
			mCreatureTarget = target.Data;
		}
	}

	public void PopulateCreaturePlay(CreatureItem creature)
	{
		Image.ReplaceTexture(creature.Form.PortraitTexture);
		DragAttackIcon.SetActive(false);
		mCreature = creature;
	}

	public void PopulateDragAttack(CreatureState creature, CreatureState target)
	{
		Image.ReplaceTexture(creature.Data.Form.PortraitTexture);
		DragAttackIcon.SetActive(true);
		mCreature = creature.Data;
		if (target != null)
		{
			mCreatureTarget = target.Data;
		}
		else
		{
			mCreatureTarget = mCreature;
		}
	}

	private void OnClick()
	{
		if (!Singleton<DWGame>.Instance.SelectingLane && !Singleton<DWBattleLane>.Instance.LootObjectsToCollect())
		{
			if (mCard != null)
			{
				Singleton<HandCardController>.Instance.OnLogCardClicked(mCard, null, mCreatureTarget);
			}
			else if (mCreature != null)
			{
				Singleton<HandCardController>.Instance.OnLogCardClicked(null, mCreature, mCreatureTarget);
			}
		}
	}

	public void FadeOut()
	{
		DisappearTween.PlayWithCallback(FadeOutDone);
	}

	private void FadeOutDone()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		Image.UnloadTexture();
	}
}

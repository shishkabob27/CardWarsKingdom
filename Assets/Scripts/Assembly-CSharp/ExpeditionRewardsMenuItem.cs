using System.Collections;
using UnityEngine;

public class ExpeditionRewardsMenuItem : MonoBehaviour
{
	private int _ItemIndex;

	private int[] _RarityRange;

	private float _HoldDuration = 3f;

	private float _CrossfadeDuration = 0.5f;

	[SerializeField]
	private UILabel _TierLabel;

	[SerializeField]
	private UILabel _SoftCurrencyLabel;

	[SerializeField]
	private UILabel _HardCurrencyLabel;

	[SerializeField]
	private UILabel _RankXPLabel;

	[SerializeField]
	private UISprite _BackgroundSprite;

	[SerializeField]
	private UITexture[] _ShardsTextures = new UITexture[2];

	[SerializeField]
	private UISprite[] _ShardFrames = new UISprite[2];

	[SerializeField]
	private UITexture[] _CakeTextures = new UITexture[2];

	[SerializeField]
	private UISprite[] _CakeFrames = new UISprite[2];

	[SerializeField]
	private UITexture[] _SpeedUpsTextures = new UITexture[2];

	[SerializeField]
	private GameObject _SpeedUpsParent;

	[SerializeField]
	private UIGrid _OuterGrid;

	[SerializeField]
	private UIGrid _InnerGrid;

	[SerializeField]
	private AnimationCurve _EaseInOutCurve;

	public void Configure(SampleExpeditionRewardsData inData, int inItemIndex)
	{
		_ItemIndex = inItemIndex;
		_RarityRange = inData.RarityRange;
		_TierLabel.text = inData.TierString;
		if (_BackgroundSprite != null)
		{
			_BackgroundSprite.color = inData.ColorPalette[0];
		}
		_SoftCurrencyLabel.gameObject.SetActive(inData.MaxSoftCurrency > 0);
		_SoftCurrencyLabel.text = KFFLocalization.Get("!!EXPEDITIONS_SOFTREWARD").Replace("<val1>", inData.MaxSoftCurrency.ToString());
		_HardCurrencyLabel.gameObject.SetActive(inData.MaxHardCurrency > 0);
		_HardCurrencyLabel.text = KFFLocalization.Get("!!EXPEDITIONS_HARDREWARD").Replace("<val1>", inData.MaxHardCurrency.ToString());
		_RankXPLabel.transform.parent.gameObject.SetActive(inData.ShouldShowRankXP);
		_ShardsTextures[0].transform.parent.gameObject.SetActive(inData.ShouldShowShardTexture);
		_CakeTextures[0].transform.parent.gameObject.SetActive(inData.ShouldShowCakeTexture);
		_SpeedUpsParent.SetActive(inData.ShouldShowSpeedUpsTexture);
		if (inData.ShouldShowShardTexture)
		{
			StartCoroutine(SetRandomShardTexture());
		}
		if (inData.ShouldShowCakeTexture)
		{
			StartCoroutine(SetRandomCakeTexture());
		}
		RepositionGrids();
		Invoke("RepositionGrids", 0.1f);
	}

	private void RepositionGrids()
	{
		_InnerGrid.Reposition();
		_OuterGrid.Reposition();
	}

	public IEnumerator SetRandomShardTexture()
	{
		UITexture texA = _ShardsTextures[0];
		UITexture texB = _ShardsTextures[1];
		UISprite frameA = _ShardFrames[0];
		UISprite frameB = _ShardFrames[1];
		EvoMaterialData evoData = EvoMaterialDataManager.Instance.GetDatabase().FindAll((EvoMaterialData m) => m.AwakenMat && m.ExpeditionDrop).RandomElement();
		texA.ReplaceTexture(evoData.UITexture);
		frameA.spriteName = evoData.Faction.CreatureShardFrameSpriteName();
		EvoMaterialData ed3 = EvoMaterialDataManager.Instance.GetDatabase().FindAll((EvoMaterialData m) => m.AwakenMat && m.ExpeditionDrop).RandomElement();
		texB.ReplaceTexture(ed3.UITexture);
		frameB.spriteName = ed3.Faction.CreatureShardFrameSpriteName();
		float initialDelay = _ItemIndex;
		yield return new WaitForSeconds(_HoldDuration + initialDelay - _CrossfadeDuration);
		TweenAlpha ta2 = texA.gameObject.AddComponent<TweenAlpha>();
		ta2.from = 1f;
		ta2.to = 0f;
		ta2.animationCurve = _EaseInOutCurve;
		ta2.duration = _CrossfadeDuration;
		ta2.PlayForward();
		yield return new WaitForSeconds(_CrossfadeDuration);
		while (base.gameObject.activeSelf)
		{
			ed3 = EvoMaterialDataManager.Instance.GetDatabase().FindAll((EvoMaterialData m) => m.AwakenMat && m.ExpeditionDrop).RandomElement();
			texA.ReplaceTexture(ed3.UITexture);
			frameA.spriteName = ed3.Faction.CreatureShardFrameSpriteName();
			yield return new WaitForSeconds(_HoldDuration - _CrossfadeDuration);
			Object.Destroy(ta2);
			ta2 = texA.gameObject.AddComponent<TweenAlpha>();
			ta2.from = 0f;
			ta2.to = 1f;
			ta2.animationCurve = _EaseInOutCurve;
			ta2.duration = _CrossfadeDuration;
			ta2.PlayForward();
			yield return new WaitForSeconds(_CrossfadeDuration);
			ed3 = EvoMaterialDataManager.Instance.GetDatabase().FindAll((EvoMaterialData m) => m.AwakenMat && m.ExpeditionDrop).RandomElement();
			texB.ReplaceTexture(ed3.UITexture);
			frameB.spriteName = ed3.Faction.CreatureShardFrameSpriteName();
			yield return new WaitForSeconds(_HoldDuration - _CrossfadeDuration);
			Object.Destroy(ta2);
			ta2 = texA.gameObject.AddComponent<TweenAlpha>();
			ta2.from = 1f;
			ta2.to = 0f;
			ta2.animationCurve = _EaseInOutCurve;
			ta2.duration = _CrossfadeDuration;
			ta2.PlayForward();
			yield return new WaitForSeconds(_CrossfadeDuration);
		}
	}

	public IEnumerator SetRandomCakeTexture()
	{
		UITexture texA = _CakeTextures[0];
		UITexture texB = _CakeTextures[1];
		UISprite frameA = _CakeFrames[0];
		UISprite frameB = _CakeFrames[1];
		XPMaterialData cakeData = XPMaterialDataManager.Instance.GetDatabase().RandomElement();
		texA.ReplaceTexture(cakeData.UITexture);
		frameA.spriteName = cakeData.Faction.CakeFrameSpriteName();
		XPMaterialData cd = GetUniqueCakeTexture(texA);
		texB.ReplaceTexture(cd.UITexture);
		frameB.spriteName = cd.Faction.CakeFrameSpriteName();
		float initialDelay = (float)_ItemIndex * 0.5f + 0.25f;
		yield return new WaitForSeconds(_HoldDuration + initialDelay - _CrossfadeDuration);
		TweenAlpha ta2 = texA.gameObject.AddComponent<TweenAlpha>();
		ta2.from = 1f;
		ta2.to = 0f;
		ta2.animationCurve = _EaseInOutCurve;
		ta2.duration = _CrossfadeDuration;
		ta2.PlayForward();
		yield return new WaitForSeconds(_CrossfadeDuration);
		while (base.gameObject.activeSelf)
		{
			XPMaterialData cd2 = GetUniqueCakeTexture(texB);
			texA.ReplaceTexture(cd2.UITexture);
			frameA.spriteName = cd2.Faction.CakeFrameSpriteName();
			yield return new WaitForSeconds(_HoldDuration - _CrossfadeDuration);
			Object.Destroy(ta2);
			ta2 = texA.gameObject.AddComponent<TweenAlpha>();
			ta2.from = 0f;
			ta2.to = 1f;
			ta2.animationCurve = _EaseInOutCurve;
			ta2.duration = _CrossfadeDuration;
			ta2.PlayForward();
			yield return new WaitForSeconds(_CrossfadeDuration);
			XPMaterialData cd3 = GetUniqueCakeTexture(texA);
			texB.ReplaceTexture(cd3.UITexture);
			frameB.spriteName = cd3.Faction.CakeFrameSpriteName();
			yield return new WaitForSeconds(_HoldDuration - _CrossfadeDuration);
			Object.Destroy(ta2);
			ta2 = texA.gameObject.AddComponent<TweenAlpha>();
			ta2.from = 1f;
			ta2.to = 0f;
			ta2.animationCurve = _EaseInOutCurve;
			ta2.duration = _CrossfadeDuration;
			ta2.PlayForward();
			yield return new WaitForSeconds(_CrossfadeDuration);
		}
	}

	private XPMaterialData GetUniqueCakeTexture(UITexture inOldTex)
	{
		XPMaterialData xPMaterialData = XPMaterialDataManager.Instance.GetDatabase().RandomElement();
		string empty = string.Empty;
		if (inOldTex != null && inOldTex.mainTexture != null)
		{
			empty = inOldTex.mainTexture.name;
		}
		while (xPMaterialData == null || xPMaterialData.UITexture == empty)
		{
			xPMaterialData = XPMaterialDataManager.Instance.GetDatabase().RandomElement();
		}
		return xPMaterialData;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}

using UnityEngine;

public class BattleHudDragCostPopup : Singleton<BattleHudDragCostPopup>
{
	public UILabel DragActionCostLabel;

	public UISprite DragActionCostIcon;

	private void Update()
	{
		UpdatePos();
	}

	public void SetCost(int amount)
	{
		DragActionCostLabel.text = amount.ToString();
		UpdatePos();
	}

	private void UpdatePos()
	{
		base.transform.position = Singleton<DWGameCamera>.Instance.BattleUICam.ScreenToWorldPoint(Input.mousePosition);
	}
}

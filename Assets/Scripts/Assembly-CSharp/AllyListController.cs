using UnityEngine;

public class AllyListController : MonoBehaviour
{
	public GameObject HelperTilePrefab;
	public UITweenController EnterInviteIdShowTween;
	public UITweenController EnterInviteIdHideTween;
	public UILabel PlayerIdLabel;
	public UILabel InviteInputLabel;
	public UILabel NoAlliesYetLabel;
	public UIInput InviteInputObject;
	public UIStreamingGrid AllyListGrid;
	public Transform SortButton;
	public UILabel AllyCountLabel;
	[SerializeField]
	private bool ShouldUseDebugDummyList;
	public bool IsHelperAssigned;
	public bool UseDummyAllies;
	public int DummyAllyListCount;
}

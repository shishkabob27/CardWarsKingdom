using UnityEngine;

public class ChatWindowController : MonoBehaviour
{
	public float TextPosWhenChatDisabled;
	public GameObject ChatEntryPrefab;
	public UITweenController ExpandTween;
	public UITweenController CollapseTween;
	public UITweenController FadeTextTween;
	public Transform CollapsedChatEntryParent;
	public Transform ExpandedChatEntryParent;
	public UIPanel CollapsedChatPanel;
	public UIPanel ExpandedChatPanel;
	public Transform ExpandedChatPanelParent;
	public UILabel InputText;
	public UIInput InputObject;
	public UILabel EntryBarLabel;
	public BoxCollider EntryBarCollider;
	public GameObject ChatWindow;
	public Transform PrefabPoolParent;
	public GameObject ExpandedBackgroundCollider;
	public GameObject EntryBar;
	public UISprite ChatIcon;
	public UISprite WindowOutline;
	public UISprite WindowFill;
	public UISprite MessageArea;
	public float SnapSpeed;
	public float TopOffset;
	public float mThrottlePoints;
	public float mThrottleMultiplier;
}

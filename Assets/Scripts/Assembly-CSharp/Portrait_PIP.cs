using UnityEngine;

public class Portrait_PIP : MonoBehaviour
{
	public int player;

	public Renderer bg;

	private Camera PIP_Camera;

	private Vector2 Size = default(Vector2);

	private Rect VPRect = default(Rect);

	private Transform mTargetTransform;

	private UIWidget mStaticPortrait;

	private Camera mBattleUICam;

	private bool mInitDone;

	public void Init()
	{
		PIP_Camera = GetComponent<Camera>();
		mBattleUICam = Singleton<DWGameCamera>.Instance.BattleUICam;
		LeaderItem leader = Singleton<DWGame>.Instance.GetLeader(player);
		mTargetTransform = ((player != 0) ? Singleton<DWGameCamera>.Instance.GetPIPTarget(leader.SelectedSkin.PIPTransformP2) : Singleton<DWGameCamera>.Instance.GetPIPTarget(leader.SelectedSkin.PIPTransformP1));
		PIP_Camera.transform.position = mTargetTransform.position;
		mStaticPortrait = Singleton<BattleHudController>.Instance.PIPIconTargets[player];
		if ((bool)mStaticPortrait)
		{
			mStaticPortrait.gameObject.SetActive(false);
		}
		QuestData currentActiveQuest = Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest;
		string text = currentActiveQuest.PortraitBg + "_Player" + (player + 1);
		Texture2D texture2D = Singleton<SLOTResourceManager>.Instance.LoadResource("Battle_Portrait_Background/" + currentActiveQuest.PortraitBg + "/" + text) as Texture2D;
		if (texture2D != null)
		{
			bg.material.mainTexture = texture2D;
		}
		mInitDone = true;
	}

	private void Update()
	{
		if (mInitDone)
		{
			Vector3[] worldCorners = mStaticPortrait.worldCorners;
			Vector3 vector = mBattleUICam.WorldToScreenPoint(worldCorners[0]);
			Vector3 vector2 = mBattleUICam.WorldToScreenPoint(worldCorners[2]);
			vector.x /= Screen.width;
			vector.y /= Screen.height;
			vector2.x /= Screen.width;
			vector2.y /= Screen.height;
			Size.x = vector2.x - vector.x;
			Size.y = vector2.y - vector.y;
			VPRect.Set(vector.x, vector.y, Size.x, Size.y);
			PIP_Camera.rect = VPRect;
			if (!PIP_Camera.GetComponent<Camera>().enabled)
			{
				PIP_Camera.GetComponent<Camera>().enabled = true;
			}
		}
	}
}

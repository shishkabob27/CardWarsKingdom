using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class TownBuildingScript : MonoBehaviour
{
	private enum NextAction
	{
		NONE,
		WAITING,
		PROCEED,
		ERROR,
		RETRY
	}

	public string BuildingId;

	public MeshRenderer mesh;

	public Transform NameBarAttachPoint;

	public Transform BadgeAttachPoint;

	public Transform ZoomNode;

	public Transform ZoomNodeWhenUnlock;

	public Animation MainBuildingAnimation;

	public Animator MainBuildingAnimator;

	public Animation[] OtherAnimations;

	private TownBuildingData mBuilding;

	private object mControllerInstance;

	private MethodInfo mPopulateMethod;

	private UITweenController mTweenController;

	private bool mWaitForUserAction;

	private NextAction mUserActionProceed;

	private PlayerSaveData.ProceedNextStep mNextFunction;

	private bool mInitializedPVP;

	private Animation mCurrentAnim;

	private bool mAnimStarted;

	public void OnClick()
	{
		if (mBuilding == null)
		{
			return;
		}
		if (!Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(BuildingId))
		{
			Singleton<TownHudController>.Instance.ShowLockedBuilding(mBuilding);
			return;
		}
		if (string.Equals(BuildingId, "TBuilding_Gacha"))
		{
			Singleton<TownController>.Instance.ShowGachaOpeningVFX(true);
		}
		SyncToTheServer();
	}

	private void SyncToTheServer()
	{
		mWaitForUserAction = true;
		mUserActionProceed = NextAction.WAITING;
		Singleton<BusyIconPanelController>.Instance.Show();
		mNextFunction = OnClickContinue;
		Singleton<PlayerInfoScript>.Instance.SaveData.User_Action(0, UserActionCallback);
	}

	public void UserActionCallback(PlayerSaveData.ActionResult result)
	{
		if (result.success)
		{
			mUserActionProceed = NextAction.PROCEED;
		}
		else
		{
			mUserActionProceed = NextAction.ERROR;
		}
	}

	private void OnClickContinue()
	{
		StartCoroutine(BringPanel(false));
		Singleton<ChatManager>.Instance.UpdateStatus();
	}

	public void ShowImmediately()
	{
		StartCoroutine(BringPanel(true));
	}

	private IEnumerator BringPanel(bool immediate)
	{
		UICamera.LockInput();
		if (!immediate)
		{
			StartClickAnimOnBuilding();
		}
		Singleton<TownHudController>.Instance.HideMainHUD();
		if (!immediate)
		{
			yield return new WaitForSeconds(0.5f);
		}
		Singleton<MouseOrbitCamera>.Instance.ZoomToButton(this);
		if (!immediate)
		{
			yield return new WaitForSeconds(Singleton<MouseOrbitCamera>.Instance.ZoomSpeed);
		}
		PlayTween();
		CallPopulateFunction();
	}

	private void Start()
	{
		if (string.Equals(BuildingId, "TBuilding_PVP"))
		{
			GameObject gameObject = base.gameObject.FindInChildren("Bld_Parent");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
				mInitializedPVP = true;
			}
		}
	}

	private void Update()
	{
		if (string.Equals(BuildingId, "TBuilding_PVP") && mInitializedPVP)
		{
			GameObject gameObject = base.gameObject.FindInChildren("Bld_Parent");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				mInitializedPVP = false;
			}
		}
		if (Singleton<TownController>.Instance.UseRealtimeLockColorUpdate)
		{
			bool unlock = Singleton<PlayerInfoScript>.Instance.IsFeatureUnlocked(BuildingId);
			BuildingLockOnMeshes(unlock);
		}
		if (!mWaitForUserAction)
		{
			return;
		}
		if (mUserActionProceed == NextAction.PROCEED)
		{
			Singleton<BusyIconPanelController>.Instance.Hide();
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			if (mNextFunction != null)
			{
				mNextFunction();
			}
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
		}
		if (mUserActionProceed == NextAction.ERROR)
		{
			mWaitForUserAction = false;
			mUserActionProceed = NextAction.NONE;
			Singleton<BusyIconPanelController>.Instance.Hide();
			Singleton<SimplePopupController>.Instance.ShowMessage(string.Empty, KFFLocalization.Get("!!SERVER_ERROR_MESSAGE"), OnCloseServerAccessErrorPopup);
		}
	}

	private void OnCloseServerAccessErrorPopup()
	{
		SyncToTheServer();
	}

	public void BuildingLockOnMeshes(bool unlock)
	{
		float value = ((!unlock) ? Singleton<TownController>.Instance.LockedDesatAmount : 0f);
		float value2 = ((!unlock) ? Singleton<TownController>.Instance.LockedAddAmount : 0f);
		Color lockedColor = Singleton<TownController>.Instance.LockedColor;
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.material.HasProperty("_EffectAmount"))
			{
				renderer.material.SetFloat("_EffectAmount", value);
			}
			if (renderer.material.HasProperty("_AdditiveAmount"))
			{
				renderer.material.SetFloat("_AdditiveAmount", value2);
			}
			if (renderer.material.HasProperty("_Color"))
			{
				renderer.material.SetColor("_Color", lockedColor);
			}
		}
		if (MainBuildingAnimation != null)
		{
			if (!unlock)
			{
				MainBuildingAnimation.Stop();
			}
			else if (MainBuildingAnimation.clip != null && MainBuildingAnimation.clip.name == "Idle")
			{
				MainBuildingAnimation.Play();
			}
		}
		else if (MainBuildingAnimator != null)
		{
			if (!unlock)
			{
				MainBuildingAnimator.StopPlayback();
			}
			else
			{
				MainBuildingAnimator.Play("Idle");
			}
		}
		Animation[] otherAnimations = OtherAnimations;
		foreach (Animation animation in otherAnimations)
		{
			if (!unlock)
			{
				animation.Stop();
			}
			else
			{
				animation.Play();
			}
		}
		ParticleSystem[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array2 = componentsInChildren2;
		foreach (ParticleSystem particleSystem in array2)
		{
			particleSystem.enableEmission = unlock;
			if (unlock)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	public void StartClickAnimOnBuilding()
	{
		if (MainBuildingAnimation != null)
		{
			MainBuildingAnimation.Play("Building_Clicked");
			mCurrentAnim = MainBuildingAnimation;
			mAnimStarted = true;
			if (MainBuildingAnimation.GetClip("Idle") != null)
			{
				MainBuildingAnimation.PlayQueued("Idle");
			}
		}
		else if (MainBuildingAnimator != null)
		{
			MainBuildingAnimator.Play("Tap");
		}
		Singleton<SLOTAudioManager>.Instance.PlaySound("SFX_BuildZoomIn");
		TweenScale componentInChildren = base.transform.GetComponentInChildren<TweenScale>();
		if (componentInChildren != null)
		{
			componentInChildren.ResetToBeginning();
			componentInChildren.Play();
		}
		if (!(mesh == null) && mesh.materials.Length == 2)
		{
			Color color = mesh.materials[1].color;
			color.a = 0.5f;
			mesh.materials[1].SetColor("_Color", color);
		}
	}

	public void StopGlimmerOnBuilding()
	{
		if (!(mesh == null) && mesh.materials.Length == 2)
		{
			Color color = mesh.materials[1].color;
			color.a = 0f;
			mesh.materials[1].SetColor("_Color", color);
		}
	}

	private void OnPress(bool pressed)
	{
		Singleton<TownController>.Instance.OnBuildingPressed(pressed);
	}

	private void PlayTween()
	{
		if (mTweenController == null)
		{
			EnableInput();
		}
		else
		{
			mTweenController.PlayWithCallback(EnableInput);
		}
	}

	private void EnableInput()
	{
		UICamera.UnlockInput();
	}

	private void CallPopulateFunction()
	{
		if (mPopulateMethod != null)
		{
			if (mBuilding.Param != null)
			{
				object[] parameters = new object[1] { mBuilding.Param };
				mPopulateMethod.Invoke(mControllerInstance, parameters);
			}
			else
			{
				mPopulateMethod.Invoke(mControllerInstance, null);
			}
		}
	}

	public void Init()
	{
		mBuilding = TownBuildingDataManager.Instance.GetData(BuildingId);
		string controllerScript = mBuilding.ControllerScript;
		if (controllerScript == string.Empty)
		{
			return;
		}
		Type type = Type.GetType(controllerScript);
		if (type == null)
		{
			return;
		}
		PropertyInfo property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		if (property == null)
		{
			return;
		}
		mControllerInstance = property.GetGetMethod().Invoke(null, null);
		mPopulateMethod = type.GetMethod("Populate");
		if (mPopulateMethod == null)
		{
			return;
		}
		string tweenController = mBuilding.TweenController;
		if (!(tweenController != string.Empty))
		{
			return;
		}
		GameObject gameObject = GameObject.Find(tweenController);
		if (!(gameObject == null))
		{
			mTweenController = gameObject.GetComponent<UITweenController>();
			if (!(mTweenController == null))
			{
			}
		}
	}
}

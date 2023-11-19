using UnityEngine;

public class TutorialObject : MonoBehaviour
{
	public string TutorialState;

	private bool mInitialized;

	private void Start()
	{
		CheckInit();
	}

	private void Update()
	{
		CheckInit();
	}

	protected void CheckInit()
	{
		if (!mInitialized && SessionManager.Instance.IsLoadDataDone() && Singleton<PlayerInfoScript>.Instance.IsInitialized)
		{
			if (TutorialState != null && TutorialState.Length > 0)
			{
				Singleton<TutorialController>.Instance.AddStateObject(this);
			}
			TutorialState tutorialState = Singleton<PlayerInfoScript>.Instance.StateData.ActiveConditionalState;
			if (tutorialState == null)
			{
				tutorialState = Singleton<PlayerInfoScript>.Instance.StateData.ActiveTutorialState;
			}
			if (tutorialState != null && tutorialState.ID == TutorialState)
			{
				UICamera.ColliderRestrictionList.Add(base.gameObject);
			}
			mInitialized = true;
		}
	}

	public static void Attach(GameObject obj, string state)
	{
		TutorialObject component = obj.GetComponent<TutorialObject>();
		if (!(component != null) || !(component.TutorialState == state))
		{
			TutorialObject tutorialObject = obj.AddComponent<TutorialObject>();
			tutorialObject.TutorialState = state;
			tutorialObject.CheckInit();
		}
	}
}

using UnityEngine;

public class TutorialDragObject : TutorialObject
{
	public new static void Attach(GameObject obj, string state)
	{
		TutorialDragObject component = obj.GetComponent<TutorialDragObject>();
		if (!(component != null) || !(component.TutorialState == state))
		{
			TutorialDragObject tutorialDragObject = obj.AddComponent<TutorialDragObject>();
			tutorialDragObject.TutorialState = state;
			tutorialDragObject.CheckInit();
		}
	}
}

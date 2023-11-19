using System.Collections.Generic;
using UnityEngine;

public class DebugGachaDrop : MonoBehaviour
{
	public GameObject debugGachaLabel;

	public UIGrid grid;

	private bool mPopulated;

	private GameObject mDebugGachaLabelObj;

	public bool populate;

	private void Start()
	{
		mDebugGachaLabelObj = grid.gameObject.InstantiateAsChild(debugGachaLabel);
	}

	private void OnEnable()
	{
		if (!mPopulated)
		{
			PopulateCreatureNameButton();
			mPopulated = true;
			Object.Destroy(mDebugGachaLabelObj);
		}
	}

	public void PopulateCreatureNameButton()
	{
		GameObject gameObject = grid.transform.InstantiateAsChild(debugGachaLabel);
		List<CreatureData> database = CreatureDataManager.Instance.GetDatabase();
		int num = 0;
		foreach (CreatureData item in database)
		{
			num++;
			if (num <= 300)
			{
				GameObject gameObject2 = grid.transform.InstantiateAsChild(debugGachaLabel);
				DebugGachaDropItem componentInChildren = gameObject2.GetComponentInChildren<DebugGachaDropItem>();
				componentInChildren.CreatureIDLabel.text = item.ID;
				componentInChildren.Creature = item;
			}
		}
		grid.Reposition();
	}

	private void Update()
	{
		if (populate)
		{
			PopulateCreatureNameButton();
			populate = false;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KFFUpsightVGController : Singleton<KFFUpsightVGController>
{
	public enum PlacementStates
	{
		PlacementPending,
		PlacementStarted,
		PlacementFinished,
		PlacementFailed
	}

	private PlacementStates _PlacementState;

	private PlacementStates PlacementState
	{
		get
		{
			return _PlacementState;
		}
		set
		{
			if (_PlacementState != value)
			{
				_PlacementState = value;
			}
		}
	}

	public void PlacementPending()
	{
		PlacementState = PlacementStates.PlacementPending;
	}

	public void PlacementStarted(string id)
	{
		PlacementState = PlacementStates.PlacementStarted;
	}

	private void Start()
	{
		PlacementPending();
	}
}

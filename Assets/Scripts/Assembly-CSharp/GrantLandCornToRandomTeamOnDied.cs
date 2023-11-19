using System.Collections.Generic;
using UnityEngine;

public class GrantLandCornToRandomTeamOnDied : OnDied
{
	public override bool OnEnable()
	{
		bool result = false;
		List<LaneState> list = base.Owner.Owner.Lanes.FindAll((LaneState m) => m != null && m.Creature != null && !m.Creature.StatusEffects.Contains((StatusState s) => s is LandCorn));
		for (int i = 0; i < base.Val1; i++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			int index = Random.Range(0, list.Count);
			ApplyStatus(list[index].Creature, StatusEnum.LandCorn, 1f);
			list.RemoveAt(index);
			result = true;
		}
		return result;
	}
}

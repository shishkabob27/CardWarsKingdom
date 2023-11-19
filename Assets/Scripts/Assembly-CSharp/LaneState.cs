using System.Collections.Generic;

public class LaneState
{
	public List<LaneState> AdjacentLanes = new List<LaneState>();

	public CreatureState Opponent
	{
		get
		{
			return OpponentLane.Creature;
		}
	}

	public int Index { get; set; }

	public PlayerState Owner { get; set; }

	public CreatureState Creature { get; set; }

	public LaneState OpponentLane { get; set; }

	public List<LaneState> ThisAndAdjacentLanes
	{
		get
		{
			List<LaneState> list = new List<LaneState>();
			list.Add(this);
			list.AddRange(AdjacentLanes);
			return list;
		}
	}

	public static LaneState Create()
	{
		return DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(LaneState)) as LaneState;
	}

	private static LaneState Create(LaneState Source)
	{
		LaneState laneState = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(LaneState)) as LaneState;
		laneState.Init(Source);
		return laneState;
	}

	public static void Destroy(LaneState State)
	{
		State.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(State);
	}

	public int Score()
	{
		if (Creature != null)
		{
			return Creature.Score();
		}
		return 0;
	}

	public void Init(LaneState Source)
	{
		if (Source.Creature != null)
		{
			Creature = Source.Creature.DeepCopy();
			Creature.Lane = this;
		}
	}

	public void Clean()
	{
		Index = 0;
		AdjacentLanes.Clear();
		Owner = null;
		OpponentLane = null;
		if (Creature != null)
		{
			CreatureState.Destroy(Creature);
			Creature = null;
		}
	}

	public LaneState DeepCopy()
	{
		return Create(this);
	}
}

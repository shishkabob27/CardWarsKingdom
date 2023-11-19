public class AIDecision
{
	public PlayerType TargetPlayer;

	public CreatureItem Creature { get; set; }

	public CardData Card { get; set; }

	public int LaneIndex1 { get; set; }

	public int LaneIndex2 { get; set; }

	public bool IsAttack { get; set; }

	public bool IsDeploy { get; set; }

	public bool EndTurn { get; set; }

	public int Seed { get; set; }

	public string TutorialForcedCardDraw { get; set; }

	public static AIDecision Create()
	{
		AIDecision aIDecision = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(AIDecision)) as AIDecision;
		aIDecision.Init();
		return aIDecision;
	}

	public static void Destroy(AIDecision Decision)
	{
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(Decision);
	}

	public void Init()
	{
		Creature = null;
		Card = null;
		LaneIndex1 = 0;
		LaneIndex2 = 0;
		EndTurn = false;
		IsAttack = false;
		Seed = 0;
		IsDeploy = false;
	}

	public override string ToString()
	{
		if (IsAttack)
		{
			return "lane " + LaneIndex1 + " attack lane " + LaneIndex2;
		}
		if (EndTurn)
		{
			return "end turn";
		}
		if (Card != null)
		{
			if (LaneIndex2 != -1)
			{
				return "play " + Card.ID + " on lane " + LaneIndex1 + " against lane " + LaneIndex2;
			}
			return "play " + Card.ID + " on lane " + LaneIndex1;
		}
		if (IsDeploy && Creature != null)
		{
			return "deploy " + Creature.Form.ID;
		}
		return "todo";
	}
}

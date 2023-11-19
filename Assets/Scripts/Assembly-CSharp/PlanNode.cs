using System.Collections.Generic;

public class PlanNode
{
	public List<PlanNode> Children = new List<PlanNode>();

	public AIDecision Decision { get; set; }

	public int Depth { get; set; }

	public int Score { get; set; }

	public BoardState State { get; set; }

	public PlanNode Parent { get; set; }

	public static PlanNode Create(PlanNode p)
	{
		PlanNode planNode = DetachedSingleton<KFFPoolManager>.Instance.GetObject(typeof(PlanNode)) as PlanNode;
		planNode.Init(p);
		return planNode;
	}

	public static void Destroy(PlanNode Node)
	{
		Node.Clean();
		DetachedSingleton<KFFPoolManager>.Instance.ReleaseObject(Node);
	}

	public void Init(PlanNode p)
	{
		Parent = p;
		if (Parent != null)
		{
			Decision = AIDecision.Create();
			State = Parent.State.DeepCopy();
			Depth = Parent.Depth + 1;
		}
		else
		{
			Depth = 0;
		}
	}

	public void Clean()
	{
		Parent = null;
		if (State != null)
		{
			BoardState.Destroy(State);
			State = null;
		}
		if (Decision != null)
		{
			AIDecision.Destroy(Decision);
			Decision = null;
		}
		Score = 0;
		foreach (PlanNode child in Children)
		{
			Destroy(child);
		}
		Children.Clear();
	}

	public void Execute()
	{
		State.ProcessMessages();
		State.ClearProcessedMessageList();
	}

	public string GeneratePrintout()
	{
		string text = null;
		for (PlanNode planNode = this; planNode != null; planNode = planNode.Parent)
		{
			if (planNode.Decision != null && !planNode.Decision.EndTurn)
			{
				text = ((text != null) ? (planNode.Decision.ToString() + " > " + text) : planNode.Decision.ToString());
			}
		}
		return text;
	}
}

using System.Collections.Generic;

public class SearchNode
{
	public PlanNode Plan;

	public int lowerbound = int.MinValue;

	public int upperbound = int.MaxValue;

	public List<SearchNode> Children = new List<SearchNode>();

	public void ResetBounds()
	{
		lowerbound = int.MinValue;
		upperbound = int.MaxValue;
	}
}

public class UpsightRequester
{
	public static void RequestContent(string requestId)
	{
		requestId = UpsightMilestoneManager.Instance.GetRequestIDWithCondition(requestId);
	}
}

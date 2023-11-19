using Allies;

public class AllyInviteStatusHolder
{
	public enum AllyCallbackMode
	{
		Accepted,
		Rejected,
		Removed
	}

	public bool IsError;

	public bool IsSuccess;

	public AllyCallbackMode Mode;

	public bool IsInviteResponseSet;

	public ResponseFlag InviteResponse = ResponseFlag.None;

	public bool IsRewardResponseSet;

	public bool IsRewardSuccess;

	public bool IsRewardError;

	public void SetStatus(bool success)
	{
		IsSuccess = success;
		IsError = !success;
	}

	public void SetInviteStatus(ResponseFlag flag)
	{
		InviteResponse = flag;
	}

	public void SetRewardStatus(bool success)
	{
		IsRewardSuccess = success;
		IsRewardError = !success;
	}
}

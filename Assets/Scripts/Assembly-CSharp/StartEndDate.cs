using System;

public class StartEndDate
{
	public DateTime StartDate;

	public DateTime EndDate;

	public bool IsWithinDates()
	{
		DateTime serverTime = TFUtils.ServerTime;
		return serverTime >= StartDate && serverTime < EndDate;
	}
}

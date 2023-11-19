// ExitGames.Client.Photon.TrafficStatsGameLevel
using System;
using ExitGames.Client.Photon;

public class TrafficStatsGameLevel
{
	private int timeOfLastDispatchCall;

	private int timeOfLastSendCall;

	public int OperationByteCount { get; set; }

	public int OperationCount { get; set; }

	public int ResultByteCount { get; set; }

	public int ResultCount { get; set; }

	public int EventByteCount { get; set; }

	public int EventCount { get; set; }

	public int LongestOpResponseCallback { get; set; }

	public byte LongestOpResponseCallbackOpCode { get; set; }

	public int LongestEventCallback { get; set; }

	public byte LongestEventCallbackCode { get; set; }

	public int LongestDeltaBetweenDispatching { get; set; }

	public int LongestDeltaBetweenSending { get; set; }

	[Obsolete("Use DispatchIncomingCommandsCalls, which has proper naming.")]
	public int DispatchCalls
	{
		get
		{
			return DispatchIncomingCommandsCalls;
		}
	}

	public int DispatchIncomingCommandsCalls { get; set; }

	public int SendOutgoingCommandsCalls { get; set; }

	public int TotalByteCount
	{
		get
		{
			return OperationByteCount + ResultByteCount + EventByteCount;
		}
	}

	public int TotalMessageCount
	{
		get
		{
			return OperationCount + ResultCount + EventCount;
		}
	}

	public int TotalIncomingByteCount
	{
		get
		{
			return ResultByteCount + EventByteCount;
		}
	}

	public int TotalIncomingMessageCount
	{
		get
		{
			return ResultCount + EventCount;
		}
	}

	public int TotalOutgoingByteCount
	{
		get
		{
			return OperationByteCount;
		}
	}

	public int TotalOutgoingMessageCount
	{
		get
		{
			return OperationCount;
		}
	}

	internal void CountOperation(int operationBytes)
	{
		OperationByteCount += operationBytes;
		OperationCount++;
	}

	internal void CountResult(int resultBytes)
	{
		ResultByteCount += resultBytes;
		ResultCount++;
	}

	internal void CountEvent(int eventBytes)
	{
		EventByteCount += eventBytes;
		EventCount++;
	}

	internal void TimeForResponseCallback(byte code, int time)
	{
		if (time > LongestOpResponseCallback)
		{
			LongestOpResponseCallback = time;
			LongestOpResponseCallbackOpCode = code;
		}
	}

	internal void TimeForEventCallback(byte code, int time)
	{
		if (time > LongestEventCallback)
		{
			LongestEventCallback = time;
			LongestEventCallbackCode = code;
		}
	}

	internal void DispatchIncomingCommandsCalled()
	{
		if (timeOfLastDispatchCall != 0)
		{
			int num = SupportClass.GetTickCount() - timeOfLastDispatchCall;
			if (num > LongestDeltaBetweenDispatching)
			{
				LongestDeltaBetweenDispatching = num;
			}
		}
		DispatchIncomingCommandsCalls++;
		timeOfLastDispatchCall = SupportClass.GetTickCount();
	}

	internal void SendOutgoingCommandsCalled()
	{
		if (timeOfLastSendCall != 0)
		{
			int num = SupportClass.GetTickCount() - timeOfLastSendCall;
			if (num > LongestDeltaBetweenSending)
			{
				LongestDeltaBetweenSending = num;
			}
		}
		SendOutgoingCommandsCalls++;
		timeOfLastSendCall = SupportClass.GetTickCount();
	}

	public void ResetMaximumCounters()
	{
		LongestDeltaBetweenDispatching = 0;
		LongestDeltaBetweenSending = 0;
		LongestEventCallback = 0;
		LongestEventCallbackCode = 0;
		LongestOpResponseCallback = 0;
		LongestOpResponseCallbackOpCode = 0;
		timeOfLastDispatchCall = 0;
		timeOfLastSendCall = 0;
	}

	public override string ToString()
	{
		return string.Format("OperationByteCount: {0} ResultByteCount: {1} EventByteCount: {2}", OperationByteCount, ResultByteCount, EventByteCount);
	}

	public string ToStringVitalStats()
	{
		return string.Format("Longest delta between Send: {0}ms Dispatch: {1}ms. Longest callback OnEv: {3}={2}ms OnResp: {5}={4}ms. Calls of Send: {6} Dispatch: {7}.", LongestDeltaBetweenSending, LongestDeltaBetweenDispatching, LongestEventCallback, LongestEventCallbackCode, LongestOpResponseCallback, LongestOpResponseCallbackOpCode, SendOutgoingCommandsCalls, DispatchIncomingCommandsCalls);
	}
}

namespace ExitGames.Client.Photon
{
	public interface IPhotonPeerListener
	{
		void DebugReturn(DebugLevel level, string message);

		void OnOperationResponse(OperationResponse operationResponse);

		void OnStatusChanged(StatusCode statusCode);

		void OnEvent(EventData eventData);
	}
}

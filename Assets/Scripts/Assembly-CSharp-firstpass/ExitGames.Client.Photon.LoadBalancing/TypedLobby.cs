namespace ExitGames.Client.Photon.LoadBalancing
{
	public class TypedLobby
	{
		public string Name;

		public LobbyType Type;

		public static readonly TypedLobby Default = new TypedLobby();

		public bool IsDefault
		{
			get
			{
				return Type == LobbyType.Default && string.IsNullOrEmpty(Name);
			}
		}

		public TypedLobby()
		{
			Name = string.Empty;
			Type = LobbyType.Default;
		}

		public TypedLobby(string name, LobbyType type)
		{
			Name = name;
			Type = type;
		}

		public override string ToString()
		{
			return string.Format("lobby '{0}'[{1}]", Name, Type);
		}
	}
}

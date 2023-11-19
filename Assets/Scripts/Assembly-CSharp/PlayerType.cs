public class PlayerType
{
	private static PlayerType user;

	private static PlayerType opponent;

	private int iValue;

	public static PlayerType User
	{
		get
		{
			if (user == null)
			{
				user = new PlayerType(0);
			}
			return user;
		}
	}

	public static PlayerType Opponent
	{
		get
		{
			if (opponent == null)
			{
				opponent = new PlayerType(1);
			}
			return opponent;
		}
	}

	public int IntValue
	{
		get
		{
			return iValue;
		}
	}

	public PlayerType(int value)
	{
		iValue = value;
	}

	public override string ToString()
	{
		if (this == User)
		{
			return "User";
		}
		return "Opponent";
	}

	public static implicit operator int(PlayerType pt)
	{
		return pt.IntValue;
	}

	public static implicit operator PlayerType(int ivalue)
	{
		if (ivalue == 0)
		{
			return User;
		}
		return Opponent;
	}

	public static PlayerType operator !(PlayerType pt)
	{
		if (pt.IntValue == 0)
		{
			return Opponent;
		}
		return User;
	}
}

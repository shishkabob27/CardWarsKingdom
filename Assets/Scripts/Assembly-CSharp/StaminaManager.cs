public class StaminaManager : DetachedSingleton<StaminaManager>
{
	public int SecondsUntilFull(StaminaType type)
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		uint num2 = ((type != 0) ? Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime : Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime);
		if (num >= num2)
		{
			return 0;
		}
		return (int)(num2 - num);
	}

	public void GetStaminaInfo(StaminaType type, out int currentStamina, out int maxStamina, out int secondsUntilNextStamina)
	{
		int num = ((type != 0) ? MiscParams.SecondsPerPvpStamina : MiscParams.SecondsPerStamina);
		if (Singleton<PlayerInfoScript>.Instance != null && Singleton<PlayerInfoScript>.Instance.RankData != null)
		{
			maxStamina = ((type != 0) ? MiscParams.MaxPvpStamina : Singleton<PlayerInfoScript>.Instance.RankData.Stamina);
		}
		else
		{
			maxStamina = 0;
		}
		int num2 = SecondsUntilFull(type);
		if (num2 > 0)
		{
			int num3 = 1 + num2 / num;
			currentStamina = maxStamina - num3;
			if (currentStamina < 0)
			{
				currentStamina = 0;
			}
			secondsUntilNextStamina = num2 % num;
		}
		else
		{
			currentStamina = maxStamina;
			secondsUntilNextStamina = 0;
		}
		uint num4 = ((type != 0) ? Singleton<PlayerInfoScript>.Instance.SaveData.ExtraPvpStamina : Singleton<PlayerInfoScript>.Instance.SaveData.ExtraQuestStamina);
		currentStamina += (int)num4;
	}

	public int GetStamina(StaminaType type)
	{
		int currentStamina;
		int maxStamina;
		int secondsUntilNextStamina;
		GetStaminaInfo(type, out currentStamina, out maxStamina, out secondsUntilNextStamina);
		return currentStamina;
	}

	public void ConsumeStamina(StaminaType type, int stamina)
	{
		if (type == StaminaType.Pvp)
		{
			return;
		}
		bool flag = true;
		if (type == StaminaType.Pvp && (uint)Singleton<PlayerInfoScript>.Instance.SaveData.ExtraPvpStamina != 0)
		{
			if ((uint)Singleton<PlayerInfoScript>.Instance.SaveData.ExtraPvpStamina >= stamina)
			{
				PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
				saveData.ExtraPvpStamina = (uint)saveData.ExtraPvpStamina - (uint)stamina;
				stamina = 0;
				flag = false;
			}
			else if (stamina > (uint)Singleton<PlayerInfoScript>.Instance.SaveData.ExtraPvpStamina)
			{
				uint num = Singleton<PlayerInfoScript>.Instance.SaveData.ExtraPvpStamina;
				stamina -= (int)num;
				Singleton<PlayerInfoScript>.Instance.SaveData.ExtraPvpStamina = 0u;
				flag = true;
			}
		}
		else if (type == StaminaType.Quests && (uint)Singleton<PlayerInfoScript>.Instance.SaveData.ExtraQuestStamina != 0)
		{
			if ((uint)Singleton<PlayerInfoScript>.Instance.SaveData.ExtraQuestStamina >= stamina)
			{
				PlayerSaveData saveData2 = Singleton<PlayerInfoScript>.Instance.SaveData;
				saveData2.ExtraQuestStamina = (uint)saveData2.ExtraQuestStamina - (uint)stamina;
				stamina = 0;
				flag = false;
			}
			else if (stamina > (uint)Singleton<PlayerInfoScript>.Instance.SaveData.ExtraQuestStamina)
			{
				uint num2 = Singleton<PlayerInfoScript>.Instance.SaveData.ExtraQuestStamina;
				stamina -= (int)num2;
				Singleton<PlayerInfoScript>.Instance.SaveData.ExtraQuestStamina = 0u;
				flag = true;
			}
		}
		if (flag)
		{
			uint num3 = TFUtils.ServerTime.UnixTimestamp();
			if (type == StaminaType.Quests)
			{
				if (num3 >= Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime)
				{
					Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime = num3 - 1;
				}
				Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime += (uint)(stamina * MiscParams.SecondsPerStamina);
			}
			else
			{
				if (num3 >= Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime)
				{
					Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime = num3 - 1;
				}
				Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime += (uint)(stamina * MiscParams.SecondsPerPvpStamina);
			}
		}
		if (type == StaminaType.Quests)
		{
			DetachedSingleton<MissionManager>.Instance.OnUseStamina(stamina);
		}
	}

	public void RestoreFixedStamina(StaminaType type, int stamina)
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		if (type == StaminaType.Quests)
		{
			if (num >= Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime)
			{
				PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
				saveData.ExtraQuestStamina = (uint)saveData.ExtraQuestStamina + (uint)stamina;
			}
			else
			{
				Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime -= (uint)(stamina * MiscParams.SecondsPerStamina);
			}
			return;
		}
		if (num >= Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime)
		{
			PlayerSaveData saveData2 = Singleton<PlayerInfoScript>.Instance.SaveData;
			saveData2.ExtraPvpStamina = (uint)saveData2.ExtraPvpStamina + (uint)stamina;
		}
		Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime -= (uint)(stamina * MiscParams.SecondsPerPvpStamina);
	}

	public void AddExtraStamina(StaminaType type, int stamina)
	{
		if (type == StaminaType.Quests)
		{
			PlayerSaveData saveData = Singleton<PlayerInfoScript>.Instance.SaveData;
			saveData.ExtraQuestStamina = (uint)saveData.ExtraQuestStamina + (uint)stamina;
		}
		else
		{
			PlayerSaveData saveData2 = Singleton<PlayerInfoScript>.Instance.SaveData;
			saveData2.ExtraPvpStamina = (uint)saveData2.ExtraPvpStamina + (uint)stamina;
		}
	}

	public void RefillStamina()
	{
		uint num = TFUtils.ServerTime.UnixTimestamp();
		Singleton<PlayerInfoScript>.Instance.SaveData.StaminaFullAtTime = num;
		Singleton<PlayerInfoScript>.Instance.SaveData.PvpStaminaFullAtTime = num;
	}
}

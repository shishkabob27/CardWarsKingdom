using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public static class PersistentAudioSettings
	{
		public const string SfxVolKey = "MA_sfxVolume";

		public const string MusicVolKey = "MA_musicVolume";

		public const string SfxMuteKey = "MA_sfxMute";

		public const string MusicMuteKey = "MA_musicMute";

		public const string BusVolKey = "MA_BusVolume_";

		public const string GroupVolKey = "MA_GroupVolume_";

		public const string BusKeysKey = "MA_BusKeys";

		public const string GroupKeysKey = "MA_GroupsKeys";

		public const string Separator = ";";

		public static string BusesUpdatedKeys
		{
			get
			{
				if (!PlayerPrefs.HasKey("MA_BusKeys"))
				{
					PlayerPrefs.SetString("MA_BusKeys", ";");
				}
				return PlayerPrefs.GetString("MA_BusKeys");
			}
			set
			{
				PlayerPrefs.SetString("MA_BusKeys", value);
			}
		}

		public static string GroupsUpdatedKeys
		{
			get
			{
				if (!PlayerPrefs.HasKey("MA_GroupsKeys"))
				{
					PlayerPrefs.SetString("MA_GroupsKeys", ";");
				}
				return PlayerPrefs.GetString("MA_GroupsKeys");
			}
			set
			{
				PlayerPrefs.SetString("MA_GroupsKeys", value);
			}
		}

		public static bool? MixerMuted
		{
			get
			{
				if (!PlayerPrefs.HasKey("MA_sfxMute"))
				{
					return null;
				}
				return PlayerPrefs.GetInt("MA_sfxMute") != 0;
			}
			set
			{
				if (!value.HasValue)
				{
					PlayerPrefs.DeleteKey("MA_sfxMute");
					return;
				}
				bool value2 = value.Value;
				PlayerPrefs.SetInt("MA_sfxMute", value2 ? 1 : 0);
				MasterAudio safeInstance = MasterAudio.SafeInstance;
				if (safeInstance != null)
				{
					MasterAudio.MixerMuted = value2;
				}
			}
		}

		public static float? MixerVolume
		{
			get
			{
				if (!PlayerPrefs.HasKey("MA_sfxVolume"))
				{
					return null;
				}
				return PlayerPrefs.GetFloat("MA_sfxVolume");
			}
			set
			{
				if (!value.HasValue)
				{
					PlayerPrefs.DeleteKey("MA_sfxVolume");
					return;
				}
				float value2 = value.Value;
				PlayerPrefs.SetFloat("MA_sfxVolume", value2);
				MasterAudio safeInstance = MasterAudio.SafeInstance;
				if (safeInstance != null)
				{
					MasterAudio.MasterVolumeLevel = value2;
				}
			}
		}

		public static bool? MusicMuted
		{
			get
			{
				if (!PlayerPrefs.HasKey("MA_musicMute"))
				{
					return null;
				}
				return PlayerPrefs.GetInt("MA_musicMute") != 0;
			}
			set
			{
				if (!value.HasValue)
				{
					PlayerPrefs.DeleteKey("MA_musicMute");
					return;
				}
				bool value2 = value.Value;
				PlayerPrefs.SetInt("MA_musicMute", value2 ? 1 : 0);
				MasterAudio safeInstance = MasterAudio.SafeInstance;
				if (safeInstance != null)
				{
					MasterAudio.PlaylistsMuted = value2;
				}
			}
		}

		public static float? MusicVolume
		{
			get
			{
				if (!PlayerPrefs.HasKey("MA_musicVolume"))
				{
					return null;
				}
				return PlayerPrefs.GetFloat("MA_musicVolume");
			}
			set
			{
				if (!value.HasValue)
				{
					PlayerPrefs.DeleteKey("MA_musicVolume");
					return;
				}
				float value2 = value.Value;
				PlayerPrefs.SetFloat("MA_musicVolume", value2);
				MasterAudio safeInstance = MasterAudio.SafeInstance;
				if (safeInstance != null)
				{
					MasterAudio.PlaylistMasterVolume = value2;
				}
			}
		}

		public static void SetBusVolume(string busName, float vol)
		{
			string key = MakeBusKey(busName);
			PlayerPrefs.SetFloat(key, vol);
			MasterAudio safeInstance = MasterAudio.SafeInstance;
			if (!(safeInstance == null))
			{
				if (MasterAudio.GrabBusByName(busName) != null)
				{
					MasterAudio.SetBusVolumeByName(busName, vol);
				}
				if (!BusesUpdatedKeys.Contains(";" + busName + ";"))
				{
					BusesUpdatedKeys = BusesUpdatedKeys + busName + ";";
				}
			}
		}

		public static string MakeBusKey(string busName)
		{
			return "MA_BusVolume_" + busName;
		}

		public static float? GetBusVolume(string busName)
		{
			string key = MakeBusKey(busName);
			if (!PlayerPrefs.HasKey(key))
			{
				return null;
			}
			return PlayerPrefs.GetFloat(key);
		}

		public static string GetGroupKey(string groupName)
		{
			return "MA_GroupVolume_" + groupName;
		}

		public static void SetGroupVolume(string grpName, float vol)
		{
			string groupKey = GetGroupKey(grpName);
			PlayerPrefs.SetFloat(groupKey, vol);
			MasterAudio safeInstance = MasterAudio.SafeInstance;
			if (!(safeInstance == null))
			{
				if (MasterAudio.GrabGroup(grpName, false) != null)
				{
					MasterAudio.SetGroupVolume(grpName, vol);
				}
				if (!GroupsUpdatedKeys.Contains(";" + grpName + ";"))
				{
					GroupsUpdatedKeys = GroupsUpdatedKeys + grpName + ";";
				}
			}
		}

		public static float? GetGroupVolume(string grpName)
		{
			string groupKey = GetGroupKey(grpName);
			if (!PlayerPrefs.HasKey(groupKey))
			{
				return null;
			}
			return PlayerPrefs.GetFloat(groupKey);
		}

		public static void RestoreMasterSettings()
		{
			if (MixerVolume.HasValue)
			{
				MasterAudio.MasterVolumeLevel = MixerVolume.Value;
			}
			if (MixerMuted.HasValue)
			{
				MasterAudio.MixerMuted = MixerMuted.Value;
			}
			if (MusicVolume.HasValue)
			{
				MasterAudio.PlaylistMasterVolume = MusicVolume.Value;
			}
			if (MusicMuted.HasValue)
			{
				MasterAudio.PlaylistsMuted = MusicMuted.Value;
			}
		}
	}
}

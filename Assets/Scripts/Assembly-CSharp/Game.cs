#define ASSERTS_ON
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class Game
{
	private class GameCrypto
	{
		public const string PlayerPrefsIndexKey = "BattleTeamInstance";

		private static readonly int[] SWIZZLE_BYTEMAP = new int[8] { 21, 9, 17, 19, 3, 28, 4, 11 };

		private static readonly string[] masks = new string[12]
		{
			"Lorem ipsum dolor sit amet", "consectetur adipisicing elit", "sed do eiusmod tempor incididunt ut labore", "et dolore magna aliqua", "Ut enim ad minim veniam", "quis nostrud exercitation ullamco laboris nisi", "ut aliquip ex ea commodo consequat", "Duis aute irure dolor in reprehenderit in voluptate ", "velit esse cillum dolore eu fugiat nulla pariatur", "Excepteur sint occaecat cupidatat non proident ",
			"sunt in culpa qui officia deserunt", "mollit anim id est laborum"
		};

		private static string GetMask()
		{
			if (PlayerPrefs.HasKey("BattleTeamInstance"))
			{
				int @int = PlayerPrefs.GetInt("BattleTeamInstance");
				SwizzleCrypto swizzleCrypto = new SwizzleCrypto(SWIZZLE_BYTEMAP);
				byte b = swizzleCrypto.DecryptByte((uint)@int);
				TFUtils.Assert(b < masks.Length, "Issue getting encryption key, index out of range");
				return masks[b];
			}
			TFUtils.Assert(false, "Don't call this all willy nilly without actually having a saved key");
			return string.Empty;
		}

		public static void EncryptAndSave(string filename, string contents, bool forceGenNewKey = false)
		{
			string empty = string.Empty;
			int num = -1;
			if (!forceGenNewKey && PlayerPrefs.HasKey("BattleTeamInstance"))
			{
				empty = GetMask();
				num = PlayerPrefs.GetInt("BattleTeamInstance");
			}
			else
			{
				int num2 = UnityEngine.Random.Range(0, masks.Length);
				empty = masks[num2];
				SwizzleCrypto swizzleCrypto = new SwizzleCrypto(SWIZZLE_BYTEMAP);
				num = (int)swizzleCrypto.Encrypt((byte)num2);
			}
			XorCrypto xorCrypto = new XorCrypto(empty);
			string data = xorCrypto.Encrypt(contents);
			TFUtils.WriteFile(filename, data);
			PlayerPrefs.SetInt("BattleTeamInstance", num);
			PlayerPrefs.Save();
		}

		public static string Decrypt(string contents)
		{
			if (PlayerPrefs.HasKey("BattleTeamInstance"))
			{
				string mask = GetMask();
				XorCrypto xorCrypto = new XorCrypto(mask);
				return xorCrypto.Decrypt(contents);
			}
			return contents;
		}
	}

	private const string GAME_FILE = "game.json";

	private string gameFile;

	private string myuserinfo;

	private string myfriendslist;

	private string myexplorerslist;

	private string myfriendrequests;

	private Version myserverversion;

	private List<string> mymessages = new List<string>();

	private List<string> mymessageslist;

	private bool _finishedAccess;

	public volatile bool needsSaveSuccessfulDialog;

	public volatile bool needsSaveFailedDialog;

	public volatile bool needsReloadErrorDialog;

	public volatile bool needsNetworkDownErrorDialog;

	public Player player;

	public bool AccessDone
	{
		get
		{
			return _finishedAccess;
		}
		set
		{
			_finishedAccess = value;
		}
	}

	public string MyUserInfo
	{
		get
		{
			return myuserinfo;
		}
		set
		{
			myuserinfo = value;
		}
	}

	public Version MyServerVersion
	{
		get
		{
			return myserverversion;
		}
		set
		{
			myserverversion = value;
		}
	}

	public List<string> MyMessages
	{
		get
		{
			return mymessages;
		}
		set
		{
			mymessages = value;
		}
	}

	public List<string> MyMessagesList
	{
		get
		{
			return mymessageslist;
		}
		set
		{
			mymessageslist = value;
		}
	}

	public string MyFriendsList
	{
		get
		{
			return myfriendslist;
		}
		set
		{
			myfriendslist = value;
		}
	}

	public string MyExplorersList
	{
		get
		{
			return myexplorerslist;
		}
		set
		{
			myexplorerslist = value;
		}
	}

	public string MyFriendRequests
	{
		get
		{
			return myfriendrequests;
		}
		set
		{
			myfriendrequests = value;
		}
	}

	public Game()
	{
		_finishedAccess = false;
	}

	public void SetPlayer(Player p)
	{
		gameFile = p.CacheFile("game.json");
		player = p;
	}

	public void LoadFromNetwork(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetGameData(session.AsyncFileResponder(key));
	}

	public void DeleteFromNetwork(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.DeleteGameData(session.AsyncFileResponder(key));
	}

	public void AssignFacebookIDToUser(string key, string facebookID, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.AssignFacebookIDToUser(session.AsyncFileResponder(key), facebookID);
	}

	public void SendMessage(string key, string to_id, string subject, string message, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.SendMessage(session.AsyncFileResponder(key), to_id, subject, message, session);
	}

	public void DeleteMessage(string key, string msg_id, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.DeleteMessage(session.AsyncFileResponder(key), msg_id, session);
	}

	public void GetMessage(string key, string id, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetMessage(session.AsyncFileResponder(key), id);
	}

	public void GetMessagesList(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetMessageList(session.AsyncFileResponder(key));
	}

	public void GetUserInfo(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetUserInfo(session.AsyncFileResponder(key));
	}

	public void GetServerVersion(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetServerVersion(session.AsyncFileResponder(key));
	}

	public void GetFriendsList(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetFriends(session.AsyncFileResponder(key));
	}

	public void GetExplorersList(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetExplorers(session.AsyncFileResponder(key));
	}

	public void GetFriendRequests(string key, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.GetFriendRequests(session.AsyncFileResponder(key));
	}

	public void ConfirmFriendRequest(string key, string id, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.ConfirmFriendRequest(session.AsyncFileResponder(key), id);
	}

	public void DenyFriendRequest(string key, string id, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.DenyFriendRequest(session.AsyncFileResponder(key), id);
	}

	public void RequestFriend(string key, string id, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.RequestFriend(session.AsyncFileResponder(key), id);
	}

	public void RemoveFriend(string key, string id, Session session)
	{
		_finishedAccess = false;
		session.WebFileServer.RemoveFriend(session.AsyncFileResponder(key), id);
	}

	public void SaveToServer(Session session, string gameData, TFWebFileServer.FileCallbackHandler callback = null)
	{
		if (gameData == null)
		{
			Debug.Log("Null gameData, not saving");
			return;
		}
		_finishedAccess = false;
		Debug.Log("Saving gamedata to server");
		TFWebFileServer.FileCallbackHandler callback2 = saveCBHandler;
		if (callback != null)
		{
			callback2 = delegate(TFWebFileResponse response)
			{
				saveCBHandler(response);
				callback(response);
			};
		}
		session.WebFileServer.SaveGameData(gameData, callback2, session);
	}

	public void saveCBHandler(TFWebFileResponse response)
	{
		if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
		{
			if (response.StatusCode == HttpStatusCode.PreconditionFailed)
			{
				Debug.Log("Reloading game from server " + response.StatusCode);
				needsReloadErrorDialog = true;
			}
			else
			{
				Debug.Log("Network down. continuing offline play without saving to server.");
				needsNetworkDownErrorDialog = true;
			}
		}
		else
		{
			Debug.Log("Game saved to server " + response.StatusCode);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				needsReloadErrorDialog = false;
				needsSaveSuccessfulDialog = true;
			}
			else
			{
				needsSaveFailedDialog = true;
			}
		}
		_finishedAccess = true;
	}

	public void SaveLocally(string json_gamestate)
	{
		if (gameFile != null)
		{
			Debug.Log("Game.SaveLocally: " + json_gamestate);
			GameCrypto.EncryptAndSave(gameFile, json_gamestate);
		}
	}

	public string LoadLocally()
	{
		string text = gameFile;
		Debug.Log("Gamefile location: " + text);
		string contents = TFUtils.ReadFile(text);
		return GameCrypto.Decrypt(contents);
	}

	public bool GameExists(Player p)
	{
		return File.Exists("game.json");
	}

	public void DestroyCache(Player p)
	{
		string text = p.CacheDir();
		if (Directory.Exists(text))
		{
			Debug.Log("Removing directory: " + text);
			Directory.Delete(text, true);
			Debug.Log("Removing file: " + p.LastPlayedFile());
			File.Delete(p.LastPlayedFile());
			if (Directory.Exists(Player.LOCAL_TEXTURE_CACHE_DIRECTORY))
			{
				Debug.Log("Removing directory: " + Player.LOCAL_TEXTURE_CACHE_DIRECTORY);
				Directory.Delete(Player.LOCAL_TEXTURE_CACHE_DIRECTORY, true);
			}
		}
	}

	public bool IsDoneServerAccess()
	{
		return _finishedAccess;
	}
}

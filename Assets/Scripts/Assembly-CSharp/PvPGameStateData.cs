using System;
using UnityEngine;

[Serializable]
public class PvPGameStateData
{
	public bool AmIPrimary;
	public bool RankedMode;
	public bool WonInitialCoinFlip;
	public string MatchId;
	public string OpponentName;
	public string OpponentID;
	public int OpponentLevel;
	public int OpponentBestLevel;
	public string OpponentFBID;
	public Texture2D OpponentPortrait;
	public string PlayerName;
	public string PlayerRank;
	public int TotalTrophies;
}

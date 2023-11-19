using UnityEngine;
using UnityEngine.Audio;

namespace DarkTonic.MasterAudio
{
	public class PlaylistController : MonoBehaviour
	{
		public bool startPlaylistOnAwake;
		public bool isShuffle;
		public bool isAutoAdvance;
		public bool loopPlaylist;
		public float _playlistVolume;
		public bool isMuted;
		public string startPlaylistName;
		public int syncGroupNum;
		public AudioMixerGroup mixerChannel;
		public MasterAudio.ItemSpatialBlendType spatialBlendType;
		public float spatialBlend;
		public bool songChangedEventExpanded;
		public string songChangedCustomEvent;
		public bool songEndedEventExpanded;
		public string songEndedCustomEvent;
	}
}

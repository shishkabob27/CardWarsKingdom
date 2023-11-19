using System.Collections;
using UnityEngine;

namespace DarkTonic.MasterAudio
{
	public class DelayBetweenSongs : MonoBehaviour
	{
		public float minTimeToWait = 1f;

		public float maxTimeToWait = 2f;

		public string playlistControllerName = "PlaylistControllerBass";

		private PlaylistController _controller;

		private void Start()
		{
			_controller = PlaylistController.InstanceByName(playlistControllerName);
			_controller.SongEnded += SongEnded;
		}

		private void OnDisable()
		{
			_controller.SongEnded -= SongEnded;
		}

		private void SongEnded(string songName)
		{
			StopAllCoroutines();
			StartCoroutine(PlaySongWithDelay());
		}

		private IEnumerator PlaySongWithDelay()
		{
			float randomTime = Random.Range(minTimeToWait, maxTimeToWait);
			if (MasterAudio.IgnoreTimeScale)
			{
				yield return StartCoroutine(CoroutineHelper.WaitForActualSeconds(randomTime));
			}
			else
			{
				yield return new WaitForSeconds(randomTime);
			}
			_controller.PlayNextSong();
		}
	}
}

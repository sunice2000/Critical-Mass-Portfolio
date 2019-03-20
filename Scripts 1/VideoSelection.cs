using UnityEngine;

using UnityEngine.SceneManagement;

using UnityEngine.Video;

using System.Collections;



public class VideoSelection : MonoBehaviour

{

	[SerializeField]

	private VideoClip videoClip;

	public static float time;

	private VideoPlayer videoPlayer;



	private void Start()

	{
		videoPlayer = FindObjectOfType<VideoPlayer>();
	}
		
	private void OnTriggerEnter(Collider collider)

	{
		//transform.position = new Vector3(-1, 0, 5);
		StartClipIfNotAlreadyPlaying();

	}



	public void StartClipIfNotAlreadyPlaying()

	{

		if (videoPlayer.clip != videoClip) {

			videoPlayer.clip = videoClip;

			videoPlayer.Play ();

		} else
			videoPlayer.Play ();

	}

	public void pauseClip()
	{
		videoPlayer.Pause();
	}

	public void playClip()
	{
		videoPlayer.Play();
	}

}
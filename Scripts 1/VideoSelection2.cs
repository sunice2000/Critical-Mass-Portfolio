using UnityEngine;

using UnityEngine.SceneManagement;

using UnityEngine.Video;

using System.Collections;



public class VideoSelection2 : MonoBehaviour

{

	[SerializeField]

	private VideoClip videoClip;

	[SerializeField]

	private VideoClip videoClip2;

	[SerializeField]

	private VideoClip videoClip3;

	[SerializeField]

	private VideoClip videoClip4;

	[SerializeField]

	private VideoClip videoClip5;

	public static float time, current;

	private VideoPlayer videoPlayer;

	private VideoClip clippy; 

	private void Start()

	{
		videoPlayer = FindObjectOfType<VideoPlayer>();
	}

	private void Update(){

	}

	private void OnTriggerEnter(Collider collider)

	{
		//transform.position = new Vector3(-1, 0, 5);
		//StartClipIfNotAlreadyPlaying();

	}



	public void StartClipIfNotAlreadyPlaying()

	{
		var setter = GazeInput.vidhit;

		if (setter == 1)
			clippy = videoClip2;
		else if (setter == 2)
			clippy = videoClip3;
		else if (setter == 3)
			clippy = videoClip4;
		else if (setter == 4)
			clippy = videoClip5;
		else
			clippy = videoClip;

		videoPlayer.clip = clippy;

		videoPlayer.Play ();

	}

	public void pauseClip()
	{
		videoPlayer.Pause();
	}

	public void playClip()
	{
		var setter = GazeInput.vidhit;

		if (setter == 1)
			clippy = videoClip2;
		else if (setter == 2)
			clippy = videoClip3;
		else if (setter == 3)
			clippy = videoClip4;
		else if (setter == 4)
			clippy = videoClip5;
		else
			clippy = videoClip;

		videoPlayer.clip = clippy;
		videoPlayer.Play();
	}

}
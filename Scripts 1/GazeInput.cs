using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GazeInput : MonoBehaviour

{
	//public static int ground;
	private GvrAudioSource intro, start, ques, entered, inside, inst1, inst2, tool1, tool2, tool3,congrat;
	private Ray ray;

	private RaycastHit hitInfo;

	private float gazeStartTime, timer, secondTimer;

	public Walk walk;

	private bool robot, initial, replay, button1, button2, buttondown, gesture;
	//private int x, y, z;
	private int max, min, counter;
	private GameObject tool, camera, ground, ground2;
	private VideoSelection lastplayed;
	private VideoSelection2 lastplayed2;
	private VideoSelection3 lastplayed3;
	private GameObject helper, message, gestures;
	public static int vidhit;
	private string current;

	[SerializeField]

	[Tooltip("The root object of all menu items")]

	private GameObject menuRoot;
//	private IEnumerator coroutine;

	void Start(){
		ground = GameObject.Find ("dirt");ground.SetActive (false);
		ground2 = GameObject.Find ("dirt2");ground2.SetActive (false);
		//GameObject.Find ("/Player/Main Camera/Tools/Trowel").SetActive(false);
		vidhit = 0;
		max = 0; 
		min = 0; 
		counter = 0;
		button1 = false;
		button2 = false;
		buttondown = false;
		camera = GameObject.Find ("Main Camera");
		tool = GameObject.Find ("Tools");
		helper = GameObject.Find ("Robot");
		message = GameObject.Find("Message");
		gestures = GameObject.Find("Gestures");
		lastplayed = null;
		gestures.SetActive (false);
		message.SetActive (false);
		GvrAudioSource[] aSources = FindObjectsOfType <GvrAudioSource> ();
		intro = aSources [5];
		start = aSources [10];
		entered = aSources [0];
		inside = aSources [3];
		inst1 = aSources [6];
		inst2 = aSources [7];
		tool1 = aSources [9];
		tool2 = aSources [1];
		tool3 = aSources [4];
		congrat = aSources [8];
		ques = aSources [2];
		//x = -1;
		//y = 0;
		//z = 10;
		robot = false;
		initial = true;
		replay = false;
		gazeStartTime = -1f;
		timer = -1f;
		secondTimer = -1f;
//		coroutine = Runner ();
//		StartCoroutine (coroutine);
		//helper.SetActive(false);
		intro.Play();
	}
	private void Update()
	{
		if (button1) {
			if (Input.GetMouseButtonDown (0)) {
				if (menuRoot.activeInHierarchy == false) {
					lastplayed2.pauseClip ();
					ShowMenu ();
				} else {
					buttondown = true;
					TrySelectMenuItem ();
				}
			} else if (ScriptExample.ScriptEngine.okay == true || ScriptExample.ScriptEngine.no == true) {
				if (ScriptExample.ScriptEngine.okay == true)
					gesture = true;
				else
					gesture = false;
				ScriptExample.ScriptEngine.okay = false;
				ScriptExample.ScriptEngine.no = false;
				TrySelectMenuItem ();
			}
		} else if (button2) {
			if (Input.GetMouseButtonDown (0)) {
				if (menuRoot.activeInHierarchy == false) {
					lastplayed3.pauseClip ();
					ShowMenu ();
				} else {
					buttondown = true;
					TrySelectMenuItem ();
				}
			} else
				TrySelectMenuItem ();		
		}else {
			if (robot) {
				TrySelectMenuItem ();
			} else if (Time.time - gazeStartTime > 28.0f && gazeStartTime != -1f) {
				ques.Play ();
				robot = true;
				helper.SetActive (true);
				message.SetActive (true);
				gestures.SetActive (true);
				//helper.transform.position = new Vector3 (x, y, z);
				//StartCoroutine (coroutine);
				lastplayed.pauseClip ();
				//start.Play ();
				/*var videoSelection = hitInfo.collider.GetComponent<VideoSelection> ();
			if (videoSelection != null) {
				lastplayed = videoSelection;
				videoSelection.pauseClip ();
			}*/
				//ShowMenu ();
			} else if (Input.GetMouseButtonDown (0)) {
				if (menuRoot.activeInHierarchy == false) {
					if (gazeStartTime != -1f) {
						secondTimer = Time.time - gazeStartTime;
						gazeStartTime = -1f;
					}
					if(lastplayed != null) {//we can change this to last played
						lastplayed.pauseClip ();
					}
					ShowMenu ();
				} else {
					TrySelectMenuItem ();

				}

			}
		}
	}


	private void TrySelectMenuItem()

	{
/*		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		else if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.name == "Capsule")
				hit.transform.position = new Vector3 (-1, 0, 5);
		}*/
		if (button1) {
			if (!buttondown) {
				if (!gesture) {
					//lastplayed2.pauseClip ();
					if (vidhit < 2)
						vidhit = 2;
					else
						vidhit = 0;
					lastplayed2.StartClipIfNotAlreadyPlaying ();
				} else {
					//lastplayed2.pauseClip ();
					if (vidhit == 1)
						vidhit = 0;
					else if (vidhit >= 4)
						vidhit = 3;
					else if (vidhit == 0)
						vidhit = 1;
					else {
						vidhit++;
						if (vidhit > 4)
							vidhit = 4;
					}
					lastplayed2.StartClipIfNotAlreadyPlaying (); 
				}
			} else {
				buttondown = false;
				ray = new Ray (transform.position, transform.forward);

				if (Physics.Raycast (ray, out hitInfo)) {
					var videoSelection = hitInfo.collider.GetComponent<VideoSelection> ();
					var videoSelection3 = hitInfo.collider.GetComponent<VideoSelection3> ();
					if (videoSelection != null) {
						helper.SetActive (false);
						helper.SetActive (true);
						gestures.SetActive (false);
						message.SetActive (false);
						start.Play ();
						initial = false;
						gazeStartTime = Time.time;
						button1 = false;
						lastplayed = videoSelection;
						videoSelection.StartClipIfNotAlreadyPlaying ();

						HideMenu ();
					} else if (videoSelection3 != null) {
						helper.SetActive (false);
						helper.SetActive (true);
						inst2.Play ();
						secondTimer = Time.time + 15f;//timer = Time.time + .5f;
						gestures.SetActive (false);
						message.SetActive (false);
						ground.SetActive (true);//GameObject.Find ("dirt").SetActive (true);
						//VideoSelection3.ground ();//GameObject.Find ("/Player/Main Camera/Tools/Trowel").SetActive (true);
						current = "Trowel";
						initial = true;
						button1 = false;
						button2 = true;
						lastplayed3 = videoSelection3;
						videoSelection3.StartClipIfNotAlreadyPlaying ();
						HideMenu ();
					} 
				} else {
					lastplayed2.StartClipIfNotAlreadyPlaying ();

					HideMenu ();
				}
			}
			} else if (button2) {
			if (buttondown) {
				buttondown = false;
				ray = new Ray (transform.position, transform.forward);

				if (Physics.Raycast (ray, out hitInfo)) {
					var videoSelection = hitInfo.collider.GetComponent<VideoSelection> ();
					var videoSelection2 = hitInfo.collider.GetComponent<VideoSelection2> ();
					if (videoSelection != null || videoSelection2 != null) {
						if (current == "Trowel") {
							GameObject.Find ("/Player/Main Camera/Tools/Trowel").SetActive (false);
							ground.SetActive (false);
						} else if (current == "Brush") {
							GameObject.Find ("/Player/Main Camera/Tools/Brush").SetActive (false);
							ground2.SetActive (false);	
						}else if (current == "Spray")
							GameObject.Find ("/Player/Main Camera/Tools/Spray").SetActive (false);
						counter = 0;
						//VideoSelection3.ground ();
					}
					if (videoSelection != null) {
						helper.SetActive (false);
						helper.SetActive (true);
						start.Play ();
						initial = false;
						gazeStartTime = Time.time;
						button1 = false;
						button2 = false;
						lastplayed = videoSelection;
						videoSelection.StartClipIfNotAlreadyPlaying ();

						HideMenu ();
					} else if (videoSelection2 != null) {
						vidhit = 0;
						helper.SetActive (false);
						helper.SetActive (true);
						message.SetActive (true);
						gestures.SetActive (true);
						inst1.Play ();
						//timer = Time.time + .5f;
						initial = true;
						button1 = true;
						button2 = false;
						lastplayed2 = videoSelection2;
						videoSelection2.StartClipIfNotAlreadyPlaying ();

						HideMenu ();
					}  
				} else {//change to last played2
					lastplayed3.StartClipIfNotAlreadyPlaying ();
					HideMenu ();
				}
			} else if (secondTimer != -1f) {
				if (secondTimer < Time.time) {
					secondTimer = -1f;
					timer = Time.time+1f;
					tool1.Play ();
					GameObject.Find ("/Player/Main Camera/Tools/Trowel").SetActive (true);
				}
			}
			else if (Time.time > timer) {
				timer = Time.time+1f;

				if (current == "Trowel") {
					float offset = camera.transform.rotation.x;
					int tools = (int)((tool.transform.rotation.x - offset) * 100);
					//if trowel
					if (tools < min)
						min = (tools);
					else if (tools > max)
						max = (int)(tools);
					else if (max - min > 30) {
						counter++;
						max = (int)tools;
						min = (int)tools;
						if (counter > 5) {
							GameObject.Find ("/Player/Main Camera/Tools/Trowel").SetActive (false);
							GameObject.Find ("/Player/Main Camera/Tools/Brush").SetActive (true);
							ground.SetActive (false);
							//VideoSelection3.ground();//GameObject.Find ("/Player/Main Camera/Tools/dirt2").SetActive (true);
							ground2.SetActive(true);
							//VideoSelection3.ground ();
							tool2.Play();
							current = "Brush";
							counter = 0;
						}
					} else {
						max = (int)tools;
						min = (int)tools;
					}
				}
				//if brush
				if (current == "Brush") {
					float offset = camera.transform.rotation.y;
					int tools = (int)((tool.transform.rotation.y - offset) * 100);
					//if trowel
					if (tools < min)
						min = (tools);
					else if (tools > max)
						max = (int)(tools);
					else if (max - min > 30) {
						counter++;
						max = (int)tools;
						min = (int)tools;
						if (counter > 5) {
							GameObject.Find ("/Player/Main Camera/Tools/Brush").SetActive (false);
							GameObject.Find ("/Player/Main Camera/Tools/Spray").SetActive (true);
							ground2.SetActive (false);
							//VideoSelection3.ground();
							tool3.Play();
							current = "Spray";
							counter = 0;
						}
					} else {
						max = (int)tools;
						min = (int)tools;
					}
				}
				//if spray
				if (current == "Spray") {
					float offset = camera.transform.rotation.y;
					int tools = (int)((tool.transform.rotation.y - offset) * 100);
					//if trowel
					if (tools < min)
						min = (tools);
					else if (tools > max)
						max = (int)(tools);
					else if (max - min > 60) {
						counter++;
						max = (int)tools;
						min = (int)tools;
						if (counter > 5) {
							GameObject.Find ("/Player/Main Camera/Tools/Spray").SetActive (false);
							current = null;
							counter = 0;
							congrat.Play ();
						}
					} else {
						max = (int)tools;
						min = (int)tools;
					}
				}
			}
		}
		else if (robot) {
			if (ScriptExample.ScriptEngine.okay == true) {//hitInfo.transform.name == "Cube") {
				ques.Stop ();
				ScriptExample.ScriptEngine.okay = false;
				gestures.SetActive (false);
				//message.SetActive(false);
				if (!replay) {
					replay = true;
					entered.Play ();
				} else {
					replay = false;
					inside.Play ();
				}
				//helper.SetActive (false);
				robot = false;

				gazeStartTime = Time.time;
				lastplayed.playClip ();
			} else if(ScriptExample.ScriptEngine.no == true){
				ques.Stop ();
				ScriptExample.ScriptEngine.no = false;
				gestures.SetActive (false);
				if (!replay) {
					replay = true;
				} else {
					replay = false;
				}
				robot = false;

				gazeStartTime = Time.time;
				lastplayed.playClip ();
			}
			//}
		} else {
			ray = new Ray (transform.position, transform.forward);

			if (Physics.Raycast (ray, out hitInfo)) {
				if (!initial) {
					if (secondTimer == -1f) {
						gazeStartTime = Time.time;
					} else
						gazeStartTime = Time.time - secondTimer;
				}
				var videoSelection = hitInfo.collider.GetComponent<VideoSelection> ();
				var videoSelection2 = hitInfo.collider.GetComponent<VideoSelection2> ();
				var videoSelection3 = hitInfo.collider.GetComponent<VideoSelection3> ();
				if (videoSelection != null) {
					button1 = false;
					if (initial) {
						helper.SetActive(false);
						helper.SetActive (true);
						start.Play ();
						initial = false;
						gazeStartTime = Time.time;
					}
					lastplayed = videoSelection;
					videoSelection.StartClipIfNotAlreadyPlaying ();

					HideMenu ();
				}
				else if (videoSelection2 != null) {
					helper.SetActive (false);
					helper.SetActive (true);
					vidhit = 0;
					inst1.Play ();
					//timer = Time.time + .5f;
					message.SetActive (true);
					gestures.SetActive(true);
					initial = true;
					button1 = true;
					button2 = false;
					lastplayed2 = videoSelection2;
					videoSelection2.StartClipIfNotAlreadyPlaying ();
					HideMenu ();
				}
				else if (videoSelection3 != null) {
					helper.SetActive (false);
					helper.SetActive (true);
					inst2.Play ();
					secondTimer = Time.time + 15f;//timer = Time.time+.5f;
					message.SetActive (false);
					gestures.SetActive (false);
					ground.SetActive (true);//GameObject.Find ("/Player/Main Camera/Tools/Trowel").SetActive (true);
					//VideoSelection3.ground();
					current = "Trowel";
					initial = true;
					button1 = false;
					button2 = true;
					lastplayed3 = videoSelection3;
					videoSelection3.StartClipIfNotAlreadyPlaying ();
					HideMenu ();
				}
			} else {
				if (!initial) {
					if (secondTimer == -1f) {
						gazeStartTime = Time.time;
					} else
						gazeStartTime = Time.time - secondTimer;
				}
				if (lastplayed != null)
					lastplayed.StartClipIfNotAlreadyPlaying ();

				HideMenu ();
			}
		}
	}

/*	private IEnumerator Runner(){
		while (true) {
			if (checker && !checker2) {
				checker2 = true;
				walk.Run ();
			}
		yield return new WaitForSeconds (0.1f); 
		//StopCoroutine (coroutine);
		}
	}*/

	private void ShowMenu()

	{

		menuRoot.SetActive(true);

	}



	private void HideMenu()

	{

		menuRoot.SetActive(false);

	}

}

public static class DeviceRotation {
	private static bool gyroInitialized = false;

	public static bool HasGyroscope {
		get {
			return SystemInfo.supportsGyroscope;
		}
	}

	public static Quaternion Get() {
		if (!gyroInitialized) {
			InitGyro();
		}

		return HasGyroscope
			? ReadGyroscopeRotation()
				: Quaternion.identity;
	}

	private static void InitGyro() {
		if (HasGyroscope) {
			Input.gyro.enabled = true;                // enable the gyroscope
			Input.gyro.updateInterval = 0.0167f;    // set the update interval to it's highest value (60 Hz)
		}
		gyroInitialized = true;
	}

	private static Quaternion ReadGyroscopeRotation() {
		return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
	}
}
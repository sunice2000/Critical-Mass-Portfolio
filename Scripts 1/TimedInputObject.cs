using System.Collections;
using UnityEngine;

public class TimedInputObject : MonoBehaviour, TimedInputHandler {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer> ().material.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void HandleTimedInput(){
		GetComponent<Renderer> ().material.color = Color.blue;
	}
}

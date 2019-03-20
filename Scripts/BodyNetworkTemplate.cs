using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkIt;

/// <summary>
/// Use this script for determining which actions to take for player movements, also used to send messages across devices using networkit
/// </summary>
public class BodyNetworkTemplate : MonoBehaviour {

    public NetworkItClient networkItClient;

    //thresholds for actions: once limit reached, will cause action
	public float ballThreshold = -8.0f;
	public float sittingThreshold = -5.0f;
	public float sleepingThreshold = -3.0f;
	public float handupThreshold = -2.0f;
	public float center = 1f;
	public float holdLit = 5;
	public float hugspace = .5f;
	public float hugspace2 = -1f;

    private List<BodyGameObject> bodies = new List<BodyGameObject>();
    private MeshRenderer mesh;

    //initialize flags
	bool start = true, standing = false, candles = true;
	bool bothsleep = false, alone = false, ball = false, sitting = false, sleeping = false, candleL = false, candleM = false, candleR = false, rested = false, stretch = true, feetup = false, hug = false, sleeping2 = false, cold = false;
	int left = 0, right = 0, middle = 0;

	void Start () {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = new Color(1.0f, 0.0f, 0.0f);

    }


    //wait for KinectManager to completely update first
    void LateUpdate () {

		if (bodies.Count == 0) {
			if (!alone) {
				alone = true;
                //for each action send a message over the network (communicate with other devices)
				Message m = new Message ("Alone");
				m.AddField ("name", "alone");
				m.DeliverToSelf = true;
				networkItClient.SendMessage (m);

				start = true;
				candles = true;
				bothsleep = false;
				ball = false;
				sitting = false;
				standing = true;
				sleeping = false;
				candleL = false;
				candleM = false;
				candleR = false;
				rested = false;
				stretch = true;
				feetup = false;
				hug = false;
				sleeping2 = false;
				cold = false;
				left = 0;
				right = 0;
				middle = 0;
			}
		}
        //Determine number of bodies, if 1 initialize to begin setup for one player

		if (bodies.Count == 1) {
			alone = false;
            //track player head, hands, feet, and body
			Vector3 spineBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.SpineBase).transform.localPosition;
			Vector3 headBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.Head).transform.localPosition;
			Vector3 handRBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.HandRight).transform.localPosition;
			Vector3 handLBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.HandLeft).transform.localPosition;
			Vector3 footRBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.FootRight).transform.localPosition;
			Vector3 footLBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.FootLeft).transform.localPosition;
			Vector3 hipBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.HipLeft).transform.localPosition;
				
			if (sitting && !candles) {//if sitting and candles are not on the screen
				if (!standing && spineBasePos.y > sittingThreshold + 2.0f) {//check if moved to standing
					standing = true;
					sitting = false;
					feetup = false;
					cold = false;
					Message m = new Message("Stand");
					m.AddField("name", "stand");
					m.DeliverToSelf = true;
					networkItClient.SendMessage(m);
				}
			}
			if (rested) { //if sleeping
				if (!stretch && handLBasePos.y > headBasePos.y && handRBasePos.y > headBasePos.y) { // check if moved to stretch position
					stretch = true;
					Message m = new Message ("Stretch");
					m.AddField ("name", "stretch");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				} 
			}
			if (start) {//if network is asleep, wake up with rising sun pose (ball->big stretch)
				if (!ball && spineBasePos.y <= ballThreshold) {
					ball = true;
					//In ball position
				} else if (ball && spineBasePos.y > sittingThreshold && handLBasePos.y > headBasePos.y && handRBasePos.y > headBasePos.y) {
					start = false;
					//Awake movement, wake up kinect network
					Message m = new Message("Awake");
					m.AddField("name", "awake");
					m.DeliverToSelf = true;
					networkItClient.SendMessage(m);
				}
			} else if (sitting) {//if sitting
				if (candles) {//and candles are on the screen
					if (handRBasePos.y >= handupThreshold && handRBasePos.z - handupThreshold < spineBasePos.z) { //see if player is lighting candle left/middle/right (with right hand)
						if (handRBasePos.x < spineBasePos.x - center) {
							left++;
							middle = 0;
							right = 0;
						} else if (handRBasePos.x > spineBasePos.x - center && handRBasePos.x < spineBasePos.x + center) {
							left = 0;
							middle++;
							right = 0;
						} else {
							left = 0;
							middle = 0;
							right++;
						}
					} else if (handLBasePos.y >= handupThreshold && handLBasePos.z - handupThreshold < spineBasePos.z)
                    { //see if player is lighting candle left/middle/right (with left hand)
                        if (handLBasePos.x < spineBasePos.x) {
							left++;
							middle = 0;
							right = 0;
						} else if (handLBasePos.x < spineBasePos.x + center && handLBasePos.x > spineBasePos.x - center) {
							left = 0;
							middle++;
							right = 0;
						} else {
							left = 0;
							middle = 0;
							right++;
						}
					}
					if (left > holdLit && !candleL) { // check if player held lighter up long enough to light a candle
						candleL = true;
						//candle left lit
						Message m = new Message ("candle left");
						m.AddField ("name", "candle left");
						m.DeliverToSelf = true;
						networkItClient.SendMessage (m);
					} else if (middle > holdLit && !candleM) {
						candleM = true;
						//candle center lit
						Message m = new Message ("candle center");
						m.AddField ("name", "candle center");
						m.DeliverToSelf = true;
						networkItClient.SendMessage (m);
					} else if (right > holdLit && !candleR) {
						candleR = true;
						//candle right lit
						Message m = new Message ("candle right");
						m.AddField ("name", "candle right");
						m.DeliverToSelf = true;
						networkItClient.SendMessage (m);
					}
					if (candleL && candleM && candleR) // check if all candles are lit
						candles = false;
				} else if (!sleeping && headBasePos.y <= sleepingThreshold) { // check if player is sleeping
					sleeping = true;
					sitting = false;
					standing = false;
					//Sleeping
					Message m = new Message ("sleep");
					m.AddField ("name", "sleep");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				} else if (!feetup && footRBasePos.y >= hipBasePos.y - 1f && footLBasePos.y >= hipBasePos.y - 1f) { // check if feet are up
					feetup = true;
                    //FeetUp
					Message m = new Message ("feetup");
					m.AddField ("name", "feetup");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				} else if (!cold && handRBasePos.x < headBasePos.x && handLBasePos.x > headBasePos.x && handRBasePos.y > headBasePos.y - 2f && handLBasePos.y > headBasePos.y - 2f) {//See if player is cold
					cold = true; 
                    //Lets warm you up
					Message m = new Message ("cold");
					m.AddField ("name", "cold");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				}

			} else if(!sitting && sleeping){
				if (spineBasePos.y <= sittingThreshold && headBasePos.y > sleepingThreshold) {//check if player is getting up from sleeping
					sitting = true;
					sleeping = false;
					rested = true;
					stretch = false;
					//Good sleep?	
					Message m = new Message("getup");
					m.AddField("name", "getup");
					m.DeliverToSelf = true;
					networkItClient.SendMessage(m);
				} 
			} else if (standing && spineBasePos.y <= sittingThreshold) {//check if player is sitting from standing
				sitting = true;
				standing = false;
				//Sitting	
				Message m = new Message("sit");
				m.AddField("name", "sit");
				m.DeliverToSelf = true;
				networkItClient.SendMessage(m);
			} else if(!standing){ // check if player is standing
				standing = true;
				Debug.Log (spineBasePos.y);
				sitting = false;
				feetup = false;
				Message m = new Message("Stand");
				m.AddField("name", "stand");
				m.DeliverToSelf = true;
				networkItClient.SendMessage(m);
				//Standing
			}

			//some bodies, send orientation update
			//multiple bodies, account for two players get each player's head, hand and body
		} else if (bodies.Count > 1) {
			alone = false;
			Vector3 spineBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.SpineBase).transform.localPosition;
			Vector3 headBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.Head).transform.localPosition;
			Vector3 handRBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.HandRight).transform.localPosition;
			Vector3 handLBasePos = bodies [0].GetJoint (Windows.Kinect.JointType.HandLeft).transform.localPosition;
			Vector3 spineBasePos2 = bodies [1].GetJoint (Windows.Kinect.JointType.SpineBase).transform.localPosition;
			Vector3 headBasePos2 = bodies [1].GetJoint (Windows.Kinect.JointType.Head).transform.localPosition;
			Vector3 handRBasePos2 = bodies [1].GetJoint (Windows.Kinect.JointType.HandRight).transform.localPosition;
			Vector3 handLBasePos2 = bodies [1].GetJoint (Windows.Kinect.JointType.HandLeft).transform.localPosition;

			if(!sitting && spineBasePos.y <= sittingThreshold && spineBasePos2.y <= sittingThreshold){ // check if both are sitting
				sitting = true;
					Message m = new Message ("sit");
					m.AddField ("name", "bothsit");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
			
			}else if(!hug) {
				if (spineBasePos.x > spineBasePos2.x) { // check if players are hugging eachother, player 1 on right
					if (handLBasePos.x < spineBasePos.x && handRBasePos.x < spineBasePos.x && handLBasePos2.x > spineBasePos2.x && handRBasePos2.x > spineBasePos2.x) {
						if (handLBasePos.x + hugspace < spineBasePos2.x || handRBasePos.x + hugspace < spineBasePos2.x && handLBasePos2.x > spineBasePos.x + hugspace || handRBasePos2.x > spineBasePos.x + hugspace) {
							hug = false;
							Debug.Log ("HUG!");
							Message m = new Message ("hug");
							m.AddField ("name", "hug");
							m.DeliverToSelf = true;
							networkItClient.SendMessage (m);
						} 
					}
				} else {
					// check if players are hugging eachother, player 1 on left
					if (handLBasePos.x > spineBasePos.x && handRBasePos.x > spineBasePos.x && handLBasePos2.x < spineBasePos2.x && handRBasePos2.x < spineBasePos2.x) {
						if (handLBasePos.x > spineBasePos2.x + hugspace2 || handRBasePos.x > spineBasePos2.x + hugspace2 && handLBasePos2.x + hugspace2 < spineBasePos.x || handRBasePos2.x + hugspace2 < spineBasePos.x) {
							hug = false;
							Debug.Log ("HUG!");
							Message m = new Message ("hug");
							m.AddField ("name", "hug");
							m.DeliverToSelf = true;
							networkItClient.SendMessage (m);
						}
					}
				}
			}
			if (sitting) {
				if (!sleeping2 && headBasePos2.y <= sleepingThreshold) {//check if player 2 is sleeping
					sleeping2 = true;
					//Goodnight player 2
					Message m = new Message ("1sleep");
					m.AddField ("name", "1sleep");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				} else if (!sleeping && headBasePos.y <= sleepingThreshold) { // check if player 1 is sleeping
					sleeping = true;
					//Goodnight player 1
					Message m = new Message ("1sleep");
					m.AddField ("name", "1sleep");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				} else if (bothsleep && headBasePos.y > sleepingThreshold && headBasePos2.y > sleepingThreshold) { // check if both players aren't sleeping
					sleeping = false;
					sleeping2 = false;
					bothsleep = false;
                    // Both players up. Get a good rest?
					Message m = new Message ("awake");
					m.AddField ("name", "awake");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				} else if (spineBasePos.y > sittingThreshold + 2.0f && spineBasePos2.y > sittingThreshold + 2.0f) { // check if both players are standing
					sitting = false;
                    // Both players are standing. Leaving so soon?
					Message m = new Message("Stand");
					m.AddField("name", "stand");
					m.DeliverToSelf = true;
					networkItClient.SendMessage(m);
				}
				if (sleeping && sleeping2 && !bothsleep) { // check if both players are sleeping
					bothsleep = true;
                    // Good morning you two, get a good rest?
					Message m = new Message ("2sleep");
					m.AddField ("name", "2sleep");
					m.DeliverToSelf = true;
					networkItClient.SendMessage (m);
				}
			}
		}
    }




    void Kinect_BodyFound(object args)
    {
        //Found player, add parts to dictionary
        BodyGameObject bodyFound = (BodyGameObject) args;
        bodies.Add(bodyFound);
    }

    void Kinect_BodyLost(object args)
    {
        //Player left, remove parts from dictionary
        ulong bodyDeletedId = (ulong) args;

        lock (bodies){
            foreach (BodyGameObject bg in bodies)
            {
                if (bg.ID == bodyDeletedId)
                {
                    bodies.Remove(bg);
                    return;
                }
            }
        }
    }

    //===================================
    //network messages

    public void NetworkIt_Message(object m)
    {
        //Retrieve message from network, which action next?
        Message message = (Message)m;

		string action = message.GetField("name");
		Debug.Log (action);
        

    }

    public void NetworkIt_Connect(object args)
    {
        //Initialize Network Connection
        EventArgs eventArgs = (EventArgs)args;
        mesh.material.color = new Color(0.0f, 1.0f, 0.0f);
    }

    public void NetworkIt_Disconnect(object args)
    {
        //Disconnect from connection
        EventArgs eventArgs = (EventArgs)args;
        mesh.material.color = new Color(1.0f, 0.0f, 0.0f);
    }

    public void NetworkIt_Error(object err)
    {
        //Error setting up network
        ErrorEventArgs error = (ErrorEventArgs)err;
        Debug.LogError(error);
    }
}

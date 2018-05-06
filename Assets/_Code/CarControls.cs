/*
Copyright 2018 Jens Andersson <jens.c.andersson@gmail.com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Control the car which the script is attached to, using input from the
// player controls with nr "playerNr". The input may come from local keyboard
// or controllers (1-4) but can always be overridden by MQTT messages to the
// topic configured in MqttController
// (which is "workshop/vehicle/uplink/<playerNr>" at the moment)
public class CarControls : MonoBehaviour {
	private readonly int MAX_POINTS_TRAILING_CAR = 10;

	public int playerNr;

	public GameObject lights;
	public GameObject rear;

	VehicleStatusUplink myCurrentState = new VehicleStatusUplink();
	VehicleStatusUplink myPreviousState = new VehicleStatusUplink();
	bool useMyCurrentState = false;

	public void ReceiveMQTTMessage(int nr, string message) {
		Debug.Log("Accepting state change: " + message);
		JsonUtility.FromJsonOverwrite(message, myCurrentState);
		Debug.Log("Accepted state change: " + message);
		useMyCurrentState = true;
	}

	void FixedUpdate () {
		MoveBasedOnUserInput();

		CarJitterAndConstrainToScreen();
		myPreviousState = JsonUtility.FromJson<VehicleStatusUplink>(JsonUtility.ToJson(myCurrentState));
	}

	float honkStarted = 0;

	// button A == honk (2/second) and lights on while holding
	bool isButtonAPressed() {
		if (playerNr == 0) {
			return Input.GetKey(KeyCode.Space);
		} else if (playerNr >= 1 && playerNr <= 4) {
			return Input.GetButton ("Honk" + playerNr);
		} else if (myCurrentState != null) {
			return myCurrentState.buttonAPressed;
		}
		return false;
	}
	// button B == hold to be able to collect up to N points
	bool isButtonBPressed() {
		if (playerNr == 0) {
			return Input.GetKey(KeyCode.LeftShift);
		} else if (playerNr >= 1 && playerNr <= 4) {
			return Input.GetButton ("B" + playerNr);
		} else if (myCurrentState != null) {
			return myCurrentState.buttonBPressed;
		}
		return false;
	}
	// up == move up
	bool isButtonUpPressed() {
		if (playerNr == 0) {
			return Input.GetKey(KeyCode.UpArrow);
		} else if (playerNr >= 1 && playerNr <= 4) {
			return (Input.GetAxis("DPadY" + playerNr) + Input.GetAxis("DPadY" + playerNr + "Alt")) < -0.1;
		} else if (myCurrentState != null) {
			return myCurrentState.buttonUpPressed;
		}
		return false;
	}
	// down == move down
	bool isButtonDownPressed() {
		if (playerNr == 0) {
			return Input.GetKey(KeyCode.DownArrow);
		} else if (playerNr >= 1 && playerNr <= 4) {
			return (Input.GetAxis("DPadY" + playerNr) + Input.GetAxis("DPadY" + playerNr + "Alt")) > 0.1;
		} else if (myCurrentState != null) {
			return myCurrentState.buttonDownPressed;
		}
		return false;
	}
	// left == brake/move left on screen
	bool isButtonLeftPressed() {
		if (playerNr == 0) {
			return Input.GetKey(KeyCode.LeftArrow);
		} else if (playerNr >= 1 && playerNr <= 4) {
			return (Input.GetAxis("DPadX" + playerNr) + Input.GetAxis("DPadX" + playerNr + "Alt")) < -0.1;
		} else if (myCurrentState != null) {
			return myCurrentState.buttonLeftPressed;
		}
		return false;
	}
	// right == accelerate/move right on screen
	bool isButtonRightPressed() {
		if (playerNr == 0) {
			return Input.GetKey(KeyCode.RightArrow);
		} else if (playerNr >= 1 && playerNr <= 4) {
			return (Input.GetAxis("DPadX" + playerNr) + Input.GetAxis("DPadX" + playerNr + "Alt")) > 0.1;
		} else if (myCurrentState != null) {
			return myCurrentState.buttonRightPressed;
		}
		return false;
	}

	void MoveBasedOnUserInput () {
		if (isButtonAPressed() && Time.time > (honkStarted + 0.5f)) {
			AudioSource audio = GetComponent<AudioSource>();
			audio.Play();
			honkStarted = Time.time;
		}
		if (isButtonAPressed()) {
			ShowLights(true);
		} else {
			ShowLights(false);
		}

		if (!isButtonBPressed()) {
			foreach (PointCollection point in points) {
				point.SetTarget(null);
			}
			points.Clear();
		}

		if (isButtonUpPressed()) {
			MoveUp();
		}
		if (isButtonDownPressed()) {
			MoveDown();
		}
		if (isButtonLeftPressed()) {
			MoveLeft();
		}
		if (isButtonRightPressed()) {
			MoveRight();
		}
	}

	void ShowLights(bool showLights) {
		if (showLights) {
			lights.SetActive(true);
		} else {
			lights.SetActive(false);
		}
	}

	void MoveUp() {
		transform.Translate(0, 0.1f/2, 0);
	}
	void MoveDown() {
		transform.Translate(0, -0.1f/2, 0);
	}
	void MoveLeft() {
		transform.Translate(-0.1f/2, 0, 0);
	}
	void MoveRight() {
		transform.Translate(0.05f/2, 0, 0);
	}

	void CarJitterAndConstrainToScreen () {
		// drop points if too far left on screen
		if (transform.position.x < -6.5) {
			foreach (PointCollection point in points) {
				point.SetTarget(null);
			}
			points.Clear();
		}

		// if outside screen boundary, push towards screen space
		if (transform.position.y > 5) {
			transform.Translate(0, -0.1f/2, 0);
		}
		if (transform.position.y < -3) {
			transform.Translate(0, 0.1f/2, 0);
		}
		if (transform.position.x > 7) {
			transform.Translate(-0.1f/2, 0, 0);
		}
		if (transform.position.x < -7) {
			transform.Translate(0.1f/2, 0, 0);
		}

		// if far outside screen boundary, push extra hard towards screen space
		if (transform.position.y > 7) {
			transform.Translate(0, -0.5f/2, 0);
		}
		if (transform.position.y < -5) {
			transform.Translate(0, 0.5f/2, 0);
		}
		if (transform.position.x > 9) {
			transform.Translate(-0.5f/2, 0, 0);
		}
		if (transform.position.x < -9) {
			transform.Translate(0.5f/2, 0, 0);
		}

		// add random movement in both x and y direction
		transform.Translate(Random.Range(-0.02f/2, 0.02f/2), Random.Range(-0.015f/2, 0.016f/2), 0);
		// add random convergence towards y == 0
		if (transform.position.y < 0) {
			transform.Translate(0, Random.Range(0, 0.01f/2), 0);
		} else {
			transform.Translate(0, Random.Range(-0.01f/2, 0), 0);
		}
		// add random rotation around Z/"forward" axis (rotate clockwise/ccw)
		transform.Rotate(Vector3.forward * Random.Range(-1.0f/2, 1.0f/2));
		// if outside [10, -5] rotation range, add random convergence towards range
		if (transform.rotation.eulerAngles.z > 10 && transform.rotation.eulerAngles.z < 180) {
			transform.Rotate(Vector3.forward * Random.Range(-4.0f/2, 0));
		}
		if (transform.rotation.eulerAngles.z < -5 ||
			(transform.rotation.eulerAngles.z > 180 && transform.rotation.eulerAngles.z < 355)) {
			transform.Rotate(Vector3.forward * Random.Range(0, 4.0f/2));
		}
	}

	// optional little tail of points following the car
	private List<PointCollection> points = new List<PointCollection>();

	// called from the items if we collide with them
	public void CollectPoint(PointCollection point) {
		Debug.Log("Point for player " + playerNr);
		if (points.Count < MAX_POINTS_TRAILING_CAR) {
			if (points.Count == 0) {
				if (rear != null) {
					point.SetTarget(rear);
				}
			} else {
				point.SetTarget(points[points.Count-1].gameObject);
			}
			points.Add(point);
		}
	}
}

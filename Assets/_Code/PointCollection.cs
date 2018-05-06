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

// For score points on screen: move left at the same rate as the
// background, and when colliding with something that has CarControls, call the
// collision method on that object with ourself as argument.
// If collected, start scaling down to a smaller version with scale 0.2.
// If in "collected" state, chase the "target" we might have, to trail the
// car which collected us (or another point already trailing it).
public class PointCollection : MonoBehaviour {
	private bool collected = false;
	private float scale = 1.0f;
	private GameObject target = null;

	public void SetTarget(GameObject g) {
		target = g;
	}

	void FixedUpdate() {
		if (collected) {
			this.transform.localScale = new Vector3(scale, scale, 1.0f);
			scale = Mathf.Clamp(scale-0.08f, 0.2f, 1.0f);
		}

		if (transform.position.x < -20) {
			Destroy(this.gameObject);
		}

		// no target? move left at background speed.
		if (target == null) {
			transform.Translate(-0.1f/2, 0, 0);
		}

		// got a target? move us 10% of the distance towards it.
		if (target != null) {
			transform.Translate(-0.05f/2, 0, 0);
			transform.Translate((target.transform.position.x-transform.position.x)/10.0f,
				(target.transform.position.y-transform.position.y)/10.0f,
				0.0f);
		}
	}

	// if picked up by a car, change state and call method on the car.
	void OnTriggerEnter(Collider other) {
		if (!collected) {
			CarControls car = other.GetComponent<CarControls>();
			if (car != null) {
	      collected = true;
				Debug.Log("Collision with " + other);
				car.CollectPoint(this);
			}
		}
	}
}

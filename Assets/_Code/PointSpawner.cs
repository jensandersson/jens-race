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

// Source for the points. Moves up and down in sinus pattern (far
// outside screen) and spawns a new point every second.
public class PointSpawner : MonoBehaviour {
	public GameObject spawnObject;
	public float lastSpawnTime = 0;

	void Start () {
		lastSpawnTime = Time.time;
	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(0.0f, 0.028f * Mathf.Sin(Time.time), 0.0f);

		if (Time.time > (lastSpawnTime + 1.0f)) {
			GameObject.Instantiate(spawnObject);
			spawnObject.transform.position = this.transform.position;
			lastSpawnTime = Time.time;
		}
	}
}

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
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Set the position to the range (-20, 0] depending on time, simulating a
// background that continuously moves to the left of the screen
public class BackgroundMoveLeft : MonoBehaviour {

	void FixedUpdate () {
		transform.position = new Vector3((Time.time * 100 * -0.1f/2) % 20, transform.position.y, transform.position.z);
	}
}

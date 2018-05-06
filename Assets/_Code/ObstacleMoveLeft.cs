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

// Move left at the same rate as the background plus an
// additional (positive) "leftSpeed".
public class ObstacleMoveLeft : MonoBehaviour {

		public float leftSpeed;

		void FixedUpdate () {
			transform.Translate(-0.1f/2 - leftSpeed/2, 0, 0);
			if (transform.position.x <-20) {
				transform.position = new Vector3(transform.position.x+80, transform.position.y, transform.position.z);
			}
		}

}

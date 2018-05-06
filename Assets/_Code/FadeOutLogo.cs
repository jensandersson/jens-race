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

// Fade alpha of target from fully opaque to fully transparent.
// Start fading after startToFadeAfter seconds and fade during fadeDuration seconds.
public class FadeOutLogo : MonoBehaviour {
	private Color origColor;
	private Color alphaColor;
	public float fadeDuration = 5.0f;
	public float startToFadeAfter = 2.0f;

	// Use this for initialization
	void Start () {
		origColor = this.GetComponent<MeshRenderer>().material.color;
		alphaColor = this.GetComponent<MeshRenderer>().material.color;
		alphaColor.a = 0;
	}

	// Update is called once per frame
	void Update () {
		this.GetComponent<MeshRenderer>().material.color =
			Color.Lerp(origColor, alphaColor, (Time.time-startToFadeAfter) / fadeDuration);

		if (Time.time > (startToFadeAfter+fadeDuration)) {
			gameObject.SetActive(false);
		}
	}
}

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
using System;
using UnityEngine;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

// Connect to MQTT endpoint and subscribe to all vehicle topics.
// Forward messages on topic "workshop/vehicle/uplink/<playerNr>" to
// vehicle <playerNr> which will deserialize them and act on it.
// Also echo messages received to "workshop/vehicle/downlink/echo/<playerNr>".
public class MqttController : MonoBehaviour {

	public CarControls[] allCars;

	private MqttClient client;

	private float myTimeCopy;

	void Update () {
		// we can't access Time.time in the MQTT receiver thread so we do it here
		myTimeCopy = Time.time;

		if (Input.GetKey(KeyCode.Escape)) {
			client.Disconnect();
			Application.Quit();
		}
	}

	void Start () {
		// enable the below for MQTT connectivity

		// NOTE: Unity currently (March 2018) cannot connect to AWS IoT endpoints due to TLS 1.2 being missing from the Unity runtime.
		// This may change soon, as a new release fixing this has been promised from the Unity developers, but for now you have to set
		// up an MQTT server yourself which accepts user-password-authentication instead of client certificates as AWS IoT requires.
		// It is, however, easy to setup a Mosquitto server which bridges everything under a particular prefix topic (such as "workshop/")
		// and connects using client certificates to AWS IoT.

/*
		// create client instance
		client = new MqttClient("mqtt-server.example.com", 1883, false, null, null, MqttSslProtocols.None);
		// register to message received
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

		string clientId = "unity-" + Guid.NewGuid().ToString();
		client.Connect(clientId, "YOURMQTTUSERNAMEGOESHERE", "YOURMQTTPASSWORDGOESHERE");

		client.Subscribe(new string[] { "workshop/vehicle/uplink/#" },
			new byte[] {
				MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
			});

		client.Publish("workshop/cloud", System.Text.Encoding.UTF8.GetBytes(@"{ ""cloudStatus"": ""ready"" }"), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
*/
	}

	int i = 0;

	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
	{
		string topic = e.Topic;
		string[] topicComponents = topic.Split('/');
		Debug.Log("Received on topic, " + (i++) + ": " + e.Topic + " : " + System.Text.Encoding.UTF8.GetString(e.Message));
		if (topicComponents.Length > 1) {
			Debug.Log("Topic component after workshop/vehicle/uplink was: " + topicComponents[3]);

			int vehicleNr = -1;
			bool result = Int32.TryParse(topicComponents[3], out vehicleNr);
			if (result) {
				Debug.Log("Parsed topic as vehicle: " + vehicleNr);
				if (vehicleNr >= 0 && vehicleNr < allCars.Length) {
					allCars[vehicleNr].ReceiveMQTTMessage(vehicleNr, System.Text.Encoding.UTF8.GetString(e.Message));

					client.Publish("workshop/vehicle/downlink/echo/" + vehicleNr, e.Message, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

					// can only access Time.time from main thread, not from here:
					string ackMessage = @"{ ""cloudTimeMillis"": " + myTimeCopy*1000 + " }";
					Debug.Log("Sending ack message: " + ackMessage);
					client.Publish("workshop/vehicle/downlink/ack/" + vehicleNr, System.Text.Encoding.UTF8.GetBytes(ackMessage), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
				}
			}
		}
	}

}

# jens-race
Unity game adaptable for up to 20 local players, provided you find enough input methods!

Source: https://github.com/jensandersson/jens-race

This simple infinity game without any way of winning can be adapted into a competitive
game and rebranded as you wish. Currently, arrow keys + shift + space provide controls
for the first player, and the next 4 players can be controlled with USB game pads using
either axis 0/1 or 4/5 + A/B buttons. Hold shift or the B button to pick up points.
Press space bar or A to honk.

It is prepared for keyboard input for player 0, USB gamepads for 1-4 and MQTT inputs for
player 5-19. That is, a total of 20 player can play provided 5-19 bring their own inputs
via MQTT. Note that client certificate connections to AWS IoT does *not* currently
(March 2018) work in Unity, although the required TLS 1.2 support is expected in the future.

All project source code under Apache License 2.0: https://www.apache.org/licenses/LICENSE-2.0

Binary library paho.mqtt.m2mqtt under Eclipse Public License 1.0: https://github.com/eclipse/paho.mqtt.m2mqtt

All non-code project assets under Creative Commons 4.0 Attribution International license: https://creativecommons.org/licenses/by/4.0

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	/// <summary>
	/// Reads inputs from the InputSystem and applies it to the PlayerController's InputCommands envelope.
	/// </summary>
	[RequireComponent(typeof(PlayerController))]
	public class PlayerInputSource : MonoBehaviour {

		InputCommands commands;
		PlayerController pc;

		private void Awake() {
			commands = new InputCommands();
			pc = GetComponent<PlayerController>();
			pc.Inputs = commands;
		}

		private void Update() {

			commands.Horizontal = Input.GetAxisRaw("Horizontal");

			if (Input.GetButtonDown("Jump")) commands.JumpDown.Set();

			commands.JumpHold = Input.GetButton("Jump");

			if (Input.GetMouseButtonDown(0)) {

				commands.BashDown.Set();

				Vector2 delta = (Vector2)CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition) - pc.BashPoint;

				if (delta == Vector2.zero) {
					commands.BashAngle = 90f; // up
				} else {
					commands.BashAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
				}

			}

		}

	}

}
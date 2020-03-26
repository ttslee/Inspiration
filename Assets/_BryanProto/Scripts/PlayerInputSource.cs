using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	/// <summary>
	/// Reads inputs from the InputSystem and applies it to the PlayerController's InputCommands envelope.
	/// </summary>
	[RequireComponent(typeof(PlayerController))]
	public class PlayerInputSource : MonoBehaviour {

		private InputCommands commands;

		private void Awake() {
			commands = new InputCommands();
			GetComponent<PlayerController>().Inputs = commands;
		}

		private void Update() {

			commands.Horizontal = Input.GetAxisRaw("Horizontal");

			if (Input.GetButtonDown("Jump")) commands.JumpDownTimestamp = Time.time;

			commands.JumpHold = Input.GetButton("Jump");
		}

	}

}
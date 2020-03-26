using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde.Bryan.Player {

	[RequireComponent(typeof(PlayerController))]
	public class PlayerInputSource : MonoBehaviour {

		private InputCommands commands;

		private void Awake() {
			commands = GetComponent<PlayerController>().Inputs;
		}

		private void Update() {
			commands.Horizontal = Input.GetAxisRaw("Horizontal");
			commands.Jump = Input.GetButtonDown("Jump");
		}

	}

}
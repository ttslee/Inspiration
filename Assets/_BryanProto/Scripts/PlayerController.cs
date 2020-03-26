using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde.Bryan.Player {

	/// <summary>
	/// Inputs to the PlayerController system.
	/// </summary>
	public class InputCommands {

		public float Horizontal { get; set; }
		public bool Jump { get; set; }
		public bool Bash { get; set; }

	}

	public class PlayerController : MonoBehaviour {

		#region Constants

		public float walkSpeed = 8f;


		Rigidbody2D body;

		#endregion

		#region Variables

		public InputCommands Inputs { get; set; }

		public enum PlayerState {
			Disabled, Movement, Bash
		}

		[SerializeField] private PlayerState m_playerState;
		public PlayerState State {
			get => m_playerState;
			private set => m_playerState = value;
		}

		#endregion

		private void Awake() {
			body = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate() {

			switch (State) {
				case PlayerState.Disabled: break;
				case PlayerState.Movement: UpdateMovement(); break;
				case PlayerState.Bash: UpdateBash(); break;
				default: State = PlayerState.Disabled; break;
			}

		}

		void UpdateMovement() {

			Vector2 vel = body.velocity;

			vel.x = walkSpeed * Inputs.Horizontal;

			body.velocity = vel;

		}

		void UpdateBash() {

		}

	}

}
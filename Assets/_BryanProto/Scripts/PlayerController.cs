using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	/// <summary>
	/// Inputs to the PlayerController system.
	/// </summary>
	public class InputCommands {

		public float Horizontal { get; set; }
		public float JumpDownTimestamp { get; set; }
		public bool JumpHold { get; set; }
		public bool Bash { get; set; }

	}

	public class PlayerController : MonoBehaviour {

		public enum PlayerState {
			Disabled, Movement, Bash
		}

		#region Constants

		[Header("Constants")]

		public float WalkSpeed = 8f;
		public float WalkAccelIn = 40f;
		public float WalkAccelOut = 50f;
		public float WalkAccelReverse = 80f;
		public const float AxisDeadzone = 0.3f;

		[Space]
		public float JumpHoldMax = 0.15f;
		public float JumpMinVel = 5f;
		public float JumpMaxVel = 8f;
		public float JumpLerpPower = 0.5f;
		public float JumpBufferTime = 0.2f;

		[Space]
		public float GravityQuick = 3f;
		public float GravitySlow = 1f;
		public float TerminalVelocity = 12f;

		[Space]
		public float GroundedCastDistance = 0.05f;
		public float GroundedCastSizeHorzDecrease = 0.02f;
		public LayerMask GroundedCastMask;


		Rigidbody2D body;
		BoxCollider2D boxCollider;

		#endregion

		#region Variables

		[Header("Variables")]

		[SerializeField] private PlayerState m_playerState;
		public PlayerState State {
			get => m_playerState;
			private set => m_playerState = value;
		}

		public InputCommands Inputs { get; set; }

		public bool grounded;

		public Vector2 velocity;

		public SimpleTimer jumpHoldTimer = new SimpleTimer(0.2f);
		public float currentGravityScale;
		public float jumpHoldProgress;

		#endregion

		private void Awake() {
			body = GetComponent<Rigidbody2D>();
			boxCollider = GetComponent<BoxCollider2D>();

			currentGravityScale = GravitySlow;
			jumpHoldTimer.Duration = JumpHoldMax;
		}

		private void Start() {
			State = PlayerState.Movement;
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.BackQuote)) {
				body.MovePosition(Vector2.zero);
				body.velocity = Vector2.zero;
			}
		}

		private void FixedUpdate() {

			UpdateGrounded();
			ApplyGravity();

			switch (State) {
				case PlayerState.Disabled: break;
				case PlayerState.Movement: UpdateMovement(); break;
				case PlayerState.Bash: UpdateBash(); break;
				default: State = PlayerState.Disabled; break;
			}

		}

		void UpdateGrounded() {

			grounded = false;

			Vector2 size = boxCollider.size * transform.localScale;
			size.x -= GroundedCastSizeHorzDecrease;

			var hits = Physics2D.BoxCastAll(transform.position, size, 0f, Vector2.down, GroundedCastDistance, GroundedCastMask);

			//Debug.Log(hits.ToCommaString(x => x.collider));

			foreach (var hit in hits) {
				if (hit.collider != null) {
					EnvironmentSurface surface = hit.collider.gameObject.GetComponent<EnvironmentSurface>();
					if (surface != null) {
						if (surface.surfaceType == SurfaceTypes.Generic) grounded = true;
					}
				}
			}
		}

		void ApplyGravity() {

			Vector2 vel = body.velocity;

			currentGravityScale = Mathf.Lerp(GravityQuick, GravitySlow, jumpHoldProgress);

			vel.y += currentGravityScale * Physics2D.gravity.y * Time.fixedDeltaTime;
			vel.y = Mathf.Max(-TerminalVelocity, vel.y);
			body.velocity = vel;

		}

		void UpdateMovement() {

			Vector2 vel = body.velocity;

			// Calculate direction of input, either -1, 0, or +1
			float inputDir = 0f;
			if (Mathf.Abs(Inputs.Horizontal) > AxisDeadzone) {
				inputDir = Mathf.Sign(Inputs.Horizontal);
			}

			// Handle snapping
			float curAccel;
			if (vel.x * inputDir < 0f) {
				curAccel = WalkAccelReverse;
			} else {
				curAccel = inputDir != 0f ? WalkAccelIn : WalkAccelOut;
			}

			// Calculate new horizontal velocity
			vel.x = Mathf.MoveTowards(vel.x, inputDir * WalkSpeed, curAccel * Time.fixedDeltaTime);

			vel.y = CalculateJump();

			body.velocity = vel;

		}

		float CalculateJump() {

			jumpHoldTimer.Update(true);

			float vy = body.velocity.y;

			// Jump continuation
			if (Inputs.JumpHold && jumpHoldTimer.Running) {
				jumpHoldProgress = Mathf.Pow(jumpHoldTimer.Progress, JumpLerpPower);
				vy = Mathf.Lerp(JumpMinVel, JumpMaxVel, jumpHoldProgress);
			}

			// Prevent regrabbing of jump after release
			if (!Inputs.JumpHold) {
				jumpHoldTimer.Stop();
			}

			// Start jump and handle buffered jumps
			if (Time.time - Inputs.JumpDownTimestamp < JumpBufferTime && grounded) {
				jumpHoldTimer.Start();
				vy = JumpMinVel;
				jumpHoldProgress = 0f;
			}

			return vy;
		}

		void UpdateBash() {

		}

	}

}
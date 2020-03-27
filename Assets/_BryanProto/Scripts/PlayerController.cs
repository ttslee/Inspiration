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
		public float WalkAccelTimeMax = 0.3f;
		public float WalkAccelIn = 80f;
		public AnimationCurve WalkAccelCurve = AnimationCurve.Linear(0, 0, 1, 1);
		public float WalkDecelMultiplier = 0.76f;
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

		public SimpleTimer jumpHoldTimer = new SimpleTimer(0.2f);
		public float currentGravityScale;
		public float jumpHoldProgress;

		#endregion

		#region Fold Temporary

		private void Awake() {
			body = GetComponent<Rigidbody2D>();
			boxCollider = GetComponent<BoxCollider2D>();

			currentGravityScale = GravitySlow;

			timerWalkIn.Duration = WalkAccelTimeMax;
			timerWalkOut.Duration = WalkAccelTimeMax;

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
				case PlayerState.Movement:
					body.velocity = new Vector2(CalculateWalk(), CalculateJump());
					break;
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

		#endregion

		#region Movement


		// CLASSES

		public enum WalkState {
			None, In, Max, Out
		}


		// VARIABLES

		public WalkState walkState;

		public SimpleTimer timerWalkIn = new SimpleTimer(0.2f);
		public SimpleTimer timerWalkOut = new SimpleTimer(0.2f);

		public int lastWalkDir = 0;


		// CODE

		float CalculateWalk() {

			// Update timers
			timerWalkIn.Update(true);
			timerWalkOut.Update(true);


			// Calculate direction of input, either -1, 0, or +1
			int inputDir = CalculateDirection(Inputs.Horizontal, AxisDeadzone);
			bool isMoving = inputDir != 0;
			bool wasMoving = lastWalkDir != 0;


			// Restarting timers if pressed / released movement from last frame
			if (isMoving && inputDir != lastWalkDir) {
				// Just pressed movement or changed directions
				timerWalkIn.Start();
				walkState = WalkState.In;
			}
			if (!isMoving && wasMoving) {
				// Just released movement
				timerWalkOut.Start();
				walkState = WalkState.Out;
			}
			lastWalkDir = inputDir;


			// Switch on current walk action

			switch (walkState) {
				case WalkState.None:

					return 0f;

				case WalkState.In:

					float vi;

					// Calculate new velocity
					if (inputDir * body.velocity.x < 0f && grounded) {

						// Reverse directions instantly
						vi = 0f;

					} else {

						// Normal movement
						vi = Mathf.MoveTowards(
							body.velocity.x,
							WalkSpeed * inputDir,
							WalkAccelCurve.Evaluate(timerWalkIn.Progress) * WalkAccelIn * Time.fixedDeltaTime);

					}


					if (Mathf.Abs(vi) == WalkSpeed) {
						walkState = WalkState.Max;
					}

					return vi;

				case WalkState.Max:

					if (Mathf.Abs(body.velocity.x) > WalkSpeed) {
						// Player is above speed cap: preserve current speed
						return body.velocity.x;
					} else {
						// Player is below speed cap: return max speed
						return inputDir * WalkSpeed;
					}

				case WalkState.Out:

					float vo = body.velocity.x * WalkDecelMultiplier;
					if (Mathf.Abs(vo) < 0.07f) vo = 0f;

					if (vo == 0f) {
						walkState = WalkState.None;
					}

					return vo;

				default:
					walkState = WalkState.None;
					return 0f;
			}

		}

		#endregion

		/// <summary>
		/// Calculate integer direction from raw value.
		/// </summary>
		int CalculateDirection(float raw, float deadzone = 0.01f) {
			if (raw < -deadzone) {
				return -1;
			} else if (raw > deadzone) {
				return 1;
			} else {
				return 0;
			}
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
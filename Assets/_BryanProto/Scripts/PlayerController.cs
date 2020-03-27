using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	public class PlayerController : MonoBehaviour {

		public enum PlayerState {
			Disabled, Movement, Bash
		}

		#region Constants

		public const float AxisDeadzone = 0.3f;

		[Header("Constants")]

		public float Gravity = -20f;
		public float TerminalVelocity = 12f;

		[Space]
		public float GroundedCastDistance = 0.05f;
		public float GroundedCastSizeHorzDecrease = 0.02f;
		public LayerMask GroundedCastMask;

		[Space]
		public float WalkSpeed = 8f;
		public float WalkAccel = 100f;
		public float WalkReduceAccel = 80f;
		public float WalkAirMultiplier = 0.65f;

		[Space]
		public float JumpHoldMax = 0.15f;
		public float JumpSpeed = 5f;
		public float JumpHorzBoost = 3f;
		public float JumpBufferTime = 0.2f;
		public float JumpCoyoteTime = 0.2f;

		[Space]
		public int BashCount = 2;
		public float BashSpeed = 20f;
		public float BashTime = 0.2f;
		public float BashEndDecrease = 4f;
		[Tooltip("Cooldown after bash before player can bash again.")]
		public float BashCooldown = 0.5f;
		[Tooltip("Cooldown until bash can be refilled if grounded.")]
		public float BashRefreshDelay = 0.2f;
		public float BashBuffer = 0.2f;

		[Space]
		public float ShakeContactGround = 0.2f;
		public float ShakeJump = 0.3f;
		public float ShakeBash = 0.7f;

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

		public Vector2 BashPoint => Position + new Vector2(0, 0.5f);
		public Vector2 Position => body.position;

		[Space]
		public Vector2 velocity;

		[Space]
		public int inputDir;
		private int lastInputDir;
		public bool grounded;

		[Space]
		public SimpleTimer jumpHoldTimer;
		public SimpleTimer jumpCoyoteTimer;

		[Space]
		public int remainingBashes;
		private Vector2 bashDir;
		public SimpleTimer bashTimer;
		public SimpleTimer bashCooldownTimer;
		public SimpleTimer bashRefreshTimer;

		#endregion

		#region Events

		// Global

		void OnContactGround() {
			Debug.Log("Contact");
			CameraController.Instance.RequestScreenShake(ShakeContactGround, Vector2.down);
		}


		// Walk

		void OnBeginWalk() { }
		void OnTurnAround() { }


		// Jump

		void OnJump() {
			Debug.Log("Jump");
			CameraController.Instance.RequestScreenShake(ShakeJump, velocity.normalized);
		}


		// Bash

		void OnBashRefill() { }
		void OnBashStart() {
			Debug.Log("Bash");
			CameraController.Instance.RequestScreenShake(ShakeBash, bashDir);
		}
		void OnBashEnd() { }

		#endregion

		#region Setup

		private void Awake() {
			body = GetComponent<Rigidbody2D>();
			boxCollider = GetComponent<BoxCollider2D>();

			jumpHoldTimer = new SimpleTimer(JumpHoldMax);
			jumpCoyoteTimer = new SimpleTimer(JumpCoyoteTime);

			bashRefreshTimer = new SimpleTimer(BashRefreshDelay);
			bashTimer = new SimpleTimer(BashTime);
			bashCooldownTimer = new SimpleTimer(BashCooldown);

			remainingBashes = BashCount;
		}

		private void Start() {
			State = PlayerState.Movement;
		}

		#endregion

		#region Update

		private void Update() {
			if (Input.GetKeyDown(KeyCode.BackQuote)) {
				body.MovePosition(Vector2.zero);
				body.velocity = Vector2.zero;
			}
		}

		private void FixedUpdate() {

			// Update timers
			jumpHoldTimer.Update(true);
			jumpCoyoteTimer.Update(true);
			bashTimer.Update(true);
			bashRefreshTimer.Update(true);
			bashCooldownTimer.Update(true);

			// Set velocity to current velocity
			velocity = body.velocity;

			// Update direction variables
			lastInputDir = inputDir;
			inputDir = Util.CalculateDirection(Inputs.Horizontal, AxisDeadzone);

			// Run global updates
			UpdateGrounded();
			UpdateBashRefill();
			CheckStartBash();

			// Execute state machine
			switch (State) {
				case PlayerState.Disabled: break;
				case PlayerState.Movement: UpdateMovement(); break;
				case PlayerState.Bash: UpdateBash(); break;
				default: State = PlayerState.Disabled; break;
			}

			// Apply calculated velocity
			body.velocity = velocity;

		}

		void UpdateGrounded() {

			bool lastGrounded = grounded;

			grounded = false;

			// Boxcast below feet to find ground
			Vector2 size = boxCollider.size * transform.localScale;
			size.x -= GroundedCastSizeHorzDecrease;
			Vector3 pos = transform.position + (Vector3)boxCollider.offset;
			var hits = Physics2D.BoxCastAll(pos, size, 0f, Vector2.down, GroundedCastDistance, GroundedCastMask);
			//Debug.Log(hits.ToCommaString(x => x.collider));

			// Search through hits for surfaces
			foreach (var hit in hits) {
				if (hit.collider != null) {
					EnvironmentSurface surface = hit.collider.gameObject.GetComponent<EnvironmentSurface>();
					if (surface != null) {
						if (surface.surfaceType == SurfaceTypes.Generic) grounded = true;
					}
				}
			}

			// Raise event on touching ground
			if (grounded && lastGrounded != grounded) {
				OnContactGround();
			}

			// Set coyote time upon leaving ground
			if (!grounded && lastGrounded != grounded) {
				jumpCoyoteTimer.Start();
			}

		}

		#endregion

		#region State - Movement

		void UpdateMovement() {

			ApplyGravity();
			Walk();
			Jump();

		}

		void ApplyGravity() {

			velocity.y += Gravity * Time.fixedDeltaTime;
			velocity.y = Mathf.Max(-TerminalVelocity, velocity.y);

		}

		void Walk() {

			// Cache previous velocity to determine if player just turned around
			float lastvx = velocity.x;

			// Reduce acceleration if airborne
			float accelMult = grounded ? 1f : WalkAirMultiplier;

			// Accelerate toward new velocity
			if (Mathf.Abs(velocity.x) > WalkSpeed && inputDir == Mathf.Sign(velocity.x)) {
				// Reduce overspeed
				velocity.x = Mathf.MoveTowards(velocity.x, inputDir * WalkSpeed, WalkReduceAccel * accelMult * Time.fixedDeltaTime);
			} else {
				// Apply normal movement
				velocity.x = Mathf.MoveTowards(velocity.x, inputDir * WalkSpeed, WalkAccel * accelMult * Time.fixedDeltaTime);
			}

			// Raise animation events
			if (grounded && inputDir != 0 && Mathf.Sign(lastvx) != Mathf.Sign(velocity.x)) {
				OnTurnAround();
			}
			if (grounded && lastInputDir == 0 && inputDir != 0) {
				OnBeginWalk();
			}

		}

		void Jump() {

			// Jump continuation
			if (Inputs.JumpHold && jumpHoldTimer.Running) {
				velocity.y = JumpSpeed;
			}

			// Prevent regrabbing of jump after release
			if (!Inputs.JumpHold) {
				jumpHoldTimer.Stop();
			}

			// Start jump and handle buffered jumps
			if (Inputs.JumpDown.Get(JumpBufferTime) && (grounded || jumpCoyoteTimer.Running)) {
				Inputs.JumpDown.Clear(); // consume buffer
				jumpHoldTimer.Start();
				velocity.y = JumpSpeed;
				velocity.x += JumpHorzBoost * inputDir;
				OnJump();
			}

		}

		#endregion

		#region State - Bash

		void UpdateBashRefill() {

			if (grounded && bashRefreshTimer.Done && remainingBashes < BashCount) {
				remainingBashes = BashCount;
				OnBashRefill();
			}

		}

		void CheckStartBash() {

			if (State == PlayerState.Movement && bashCooldownTimer.Done && remainingBashes > 0 && Inputs.BashDown.Get(BashBuffer)) {

				Inputs.BashDown.Clear();

				--remainingBashes;
				bashDir = new Vector2(Mathf.Cos(Inputs.BashAngle), Mathf.Sin(Inputs.BashAngle));
				bashTimer.Start();
				bashRefreshTimer.Start();
				OnBashStart();

				State = PlayerState.Bash;

			}

		}

		void UpdateBash() {

			if (bashTimer.Running) {
				velocity = BashSpeed * bashDir;
			} else {
				bashCooldownTimer.Start();
				velocity = Vector2.MoveTowards(velocity, Vector2.zero, BashEndDecrease);
				OnBashEnd();
				State = PlayerState.Movement;
			}

		}

		#endregion

	}

}
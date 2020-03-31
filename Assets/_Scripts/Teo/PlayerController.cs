using System;
using System.Collections.Generic;
using UnityEngine;
using Engarde_Bryan;
using Engarde_Bryan.Player;

namespace Engarde_Teo.Player
{

    public class PlayerController : MonoBehaviour
    {

        public enum PlayerState
        {
            Disabled, Movement, Bash
        }

        #region Constants

        public const float AxisDeadzone = 0.3f;

        [Header("Constants")]

        public float Gravity = -20f;
        public float TerminalVelocity = 12f;
        public float OverspeedDecel = 35f;

        [Space]
        public float ReflectNormalY = 0.4f;
        public float ReflectMove = 5f;
        public Vector2 ReflectBounce = new Vector2(1, 0.3f);

        [Space]
        public float WalkSpeed = 8f;
        public float WalkAccel = 100f;
        public float WalkAirMultiplier = 0.65f;

        [Space]
        public float JumpHoldMax = 0.15f;
        public float JumpSpeed = 5f;
        public float JumpHorzBoost = 3f;
        public float JumpBufferTime = 0.2f;
        public float JumpCoyoteTime = 0.2f;
        public float JumpDelayTime = 0.5f;

        [Space]
        public bool BashUnlimited;
        public int BashCount = 2;
        public float BashAngleClamp = 15f;
        public float BashSpeed = 20f;
        public float BashTime = 0.2f;
        public float BashEndDecrease = 4f;
        public float BashCooldown = 0.5f;
        public float BashRefreshDelay = 0.2f;
        public float BashBuffer = 0.2f;

        [Space]
        public float BoundSpeed = 5f;
        public float BoundHorzSpeed = 16f;
        public float BoundWindow = 0.4f;
        public float MiniBoundHorzBoost = 8f;
        public float BoundXPower = 0.25f;

        [Space]
        public float ShakeBash = 0.7f;

        Rigidbody2D body;
        SpriteRenderer[] sprites;

        [Space]
        public Rigidbody2D limbHead;
        public Rigidbody2D limbBody;
        public LimbController legUR;
        public LimbController legR;
        public LimbController legUL;
        public LimbController legL;

        //PlayerAnimator animator;

        #endregion

        #region Variables

        [Header("Variables")]

        [SerializeField] private PlayerState m_playerState;
        public PlayerState State
        {
            get => m_playerState;
            private set => m_playerState = value;
        }

        [Space]
        public bool logEvents;

        public InputCommands Inputs { get; set; }

        public Vector2 BashPoint => Position + new Vector2(0, 0.5f);
        public Vector2 Position => body.position;

        [Space]
        public Vector2 velocity;

        [Space]
        public int inputDir;
        private int lastInputDir;

        private GroundCheck groundScript;
        public bool grounded;
        public bool reflectFlag;
        public Vector2 reflectNormal;

        [Space]
        public float curJumpSpeed;
        public SimpleTimer jumpHoldTimer;
        public SimpleTimer jumpCoyoteTimer;
        public SimpleTimer jumpDelayTimer;

        [Space]
        public int remainingBashes;
        public Vector2 bashNormal;
        private int bashDir;
        public SimpleTimer bashTimer;
        public SimpleTimer bashCooldownTimer;
        public SimpleTimer bashRefreshTimer;

        [Space]
        public SimpleTimer boundWindowTimer;

        #endregion

        #region Events

        // Global

        void OnContactGround()
        {
            if (logEvents) Debug.Log("Contact");
            //CameraController.Instance.RequestScreenShake(ShakeContactGround, Vector2.down);
        }

        // Walk

        void OnBeginWalk()
        {
            if (logEvents) Debug.Log("Begin walk");
        }


        // Jump

        void OnJump()
        {
            if (logEvents) Debug.Log("Jump");
            //Debug.Log("Jump");
            jumpDelayTimer.Start();
        }

        void OnSuperJump()
        {
            if (logEvents) Debug.Log("Super Jump");
            //Debug.Log("Super Jump");
        }

        // Bash

        void OnBashRefill()
        {
            if (logEvents) Debug.Log("Bash Refill");
        }
        void OnBashStart()
        {
            if (logEvents) Debug.Log("Bash Start");
            //Debug.Log("Bash Start");
            CameraController.Instance.RequestScreenShake(ShakeBash, bashNormal);
            //animator.BeginTrail();
        }
        void OnBashEnd()
        {
            if (logEvents) Debug.Log("Bash End");
            //Debug.Log("Bash End");
            // animator.EndTrail();
        }

        #endregion

        #region Setup

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            groundScript = GetComponent<GroundCheck>();
            //animator = GetComponent<PlayerAnimator>();

            jumpHoldTimer = new SimpleTimer(JumpHoldMax);
            jumpCoyoteTimer = new SimpleTimer(JumpCoyoteTime);
            jumpDelayTimer = new SimpleTimer(JumpDelayTime);


            bashRefreshTimer = new SimpleTimer(BashRefreshDelay);
            bashTimer = new SimpleTimer(BashTime);
            bashCooldownTimer = new SimpleTimer(BashCooldown);

            boundWindowTimer = new SimpleTimer(BoundWindow);

            remainingBashes = BashCount;

            sprites = GetComponentsInChildren<SpriteRenderer>();
            SetColor();
            CameraController.Instance.trackTarget = transform;
        }

        private void SetColor()
        {
            //Color of Stickman
            Color newColor = new Color(UnityEngine.Random.Range(0f, 0.7f), UnityEngine.Random.Range(0f, 0.7f), UnityEngine.Random.Range(0f, 0.7f));
            foreach (SpriteRenderer sprite in sprites)
                sprite.color = newColor;
        }

        private void Start()
        {
            State = PlayerState.Movement;
        }

        #endregion

        #region Update

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                body.MovePosition(Vector2.zero);
                body.velocity = Vector2.zero;
                velocity = Vector2.zero;
                transform.position = Vector3.zero;
            }

            //animator.Animate();
        }

        private void FixedUpdate()
        {

            // Update timers
            jumpHoldTimer.Update(true);
            jumpCoyoteTimer.Update(true);
            jumpDelayTimer.Update(true);
            bashTimer.Update(true);
            bashRefreshTimer.Update(true);
            bashCooldownTimer.Update(true);
            boundWindowTimer.Update(true);

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
            switch (State)
            {
                case PlayerState.Disabled: break;
                case PlayerState.Movement: UpdateMovement(); break;
                case PlayerState.Bash: UpdateBash(); break;
                default: State = PlayerState.Disabled; break;
            }

            //UpdateReflect();

            // Apply calculated velocity
            body.velocity = velocity;

            //animator.AnimateFixed();

        }

        void UpdateGrounded()
        {

            bool lastGrounded = grounded;

            grounded = false;

            // Search through hits for surfaces // NOW USES LAYERS
            if (groundScript.IsGrounded() && bashCooldownTimer.Done)
            {
                grounded = true;
                limbBody.MoveRotation(90 + 50 * Time.fixedDeltaTime);
                DisableLegs(false);

                //Maybe also add a ground effect here or only when hes directly skidding against the ground
            }
            else
            {
                DisableLegs(true);
            }

            // Raise event on touching ground
            if (grounded && lastGrounded != grounded)
            {
                OnContactGround();
            }

            // Set coyote time upon leaving ground
            if (!grounded && lastGrounded != grounded)
            {
                jumpCoyoteTimer.Start();
            }

        }

        private void DisableLegs(bool check)
        {
            legUL.enabled = !check;
            legR.enabled = !check;
            legUR.enabled = !check;
            legR.enabled = !check;
        }

        void UpdateReflect()
        {
            if (reflectFlag)
            {
                reflectFlag = false;

                float curDir = Mathf.Sign(velocity.x);

                bashDir *= -1;
                bashNormal.x *= -1;
                velocity.x *= -1;

                Vector2 bounce = ReflectBounce;
                bounce.x *= curDir;
                velocity += bounce;

                Debug.Log(velocity);

                body.MovePosition(body.position + reflectNormal * ReflectMove);
            }
        }

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    EnvironmentSurface surface = collision.gameObject.GetComponent<EnvironmentSurface>();
        //    if (surface != null)
        //    {
        //        if (surface.surfaceType == SurfaceTypes.Generic)
        //        {

        //            foreach (ContactPoint2D pt in collision.contacts)
        //            {

        //                if (Mathf.Abs(pt.normal.y) < ReflectNormalY)
        //                {

        //                    reflectFlag = true;
        //                    reflectNormal = pt.normal;

        //                    //Debug.Log("Reflect");

        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Movement

        void UpdateMovement()
        {

            ApplyGravity();
            Walk();
            UpdateJump();
            AnyJump();

        }

        void ApplyGravity()
        {

            if (velocity.y > JumpSpeed)
            {
                velocity.y = Mathf.MoveTowards(velocity.y, 0f, OverspeedDecel * Time.fixedDeltaTime);
            }

            velocity.y += Gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(-TerminalVelocity, velocity.y);

        }

        void Walk()
        {

            // Reduce acceleration if airborne
            float accelMult = grounded ? 1f : WalkAirMultiplier;

            // Accelerate toward new velocity
            if (Mathf.Abs(velocity.x) > WalkSpeed && inputDir == Mathf.Sign(velocity.x))
            {
                // Reduce overspeed
                velocity.x = Mathf.MoveTowards(velocity.x, inputDir * WalkSpeed, OverspeedDecel * accelMult * Time.fixedDeltaTime);
            }
            else
            {
                // Apply normal movement
                velocity.x = Mathf.MoveTowards(velocity.x, inputDir * WalkSpeed, WalkAccel * accelMult * Time.fixedDeltaTime);
            }

            if (grounded && lastInputDir == 0 && inputDir != 0)
            {
                OnBeginWalk();
            }

        }

        #endregion

        #region Jumps

        void UpdateJump()
        {

            // Jump continuation
            if (Inputs.JumpHold && jumpHoldTimer.Running)
            {
                velocity.y = curJumpSpeed;
            }

            // Prevent regrabbing of jump after release
            if (!Inputs.JumpHold)
            {
                jumpHoldTimer.Stop();
            }

        }

        void AnyJump()
        {

            if (inputDir != -bashDir && boundWindowTimer.Running)
            {
                // Bound if in bound window and controller neutral / inline with bash direction
                Bound();
            }
            else
            {
                // Jump in all other scenarios
                Jump();
            }

        }

        void Jump()
        {

            // Start jump and handle buffered jumps
            if (jumpDelayTimer.Done && Inputs.JumpDown.Get(JumpBufferTime) && (grounded || jumpCoyoteTimer.Running))
            {

                Inputs.JumpDown.Clear(); // consume buffer

                jumpHoldTimer.Start();
                jumpDelayTimer.Start();

                // Set vertical jump speed
                curJumpSpeed = JumpSpeed;
                velocity.y = curJumpSpeed;

                // Apply Horizontal boost
                if (boundWindowTimer.Running && inputDir == 0)
                {
                    velocity.x += MiniBoundHorzBoost * bashDir;
                }
                else
                {
                    velocity.x += JumpHorzBoost * inputDir;
                }

                OnJump();

                State = PlayerState.Movement;

            }

        }

        void Bound()
        {

            // Start jump and handle buffered jumps
            if (Inputs.JumpDown.Get(JumpBufferTime) && grounded)
            {

                Inputs.JumpDown.Clear(); // consume buffer

                jumpHoldTimer.Start();

                curJumpSpeed = BoundSpeed;

                velocity.y = curJumpSpeed;
                velocity.x = Mathf.Pow(Mathf.Abs(bashNormal.x), BoundXPower) * bashDir * BoundHorzSpeed;

                OnSuperJump();

                State = PlayerState.Movement;

            }

        }

        #endregion

        #region Bash

        void UpdateBashRefill()
        {

            if (grounded && bashRefreshTimer.Done && remainingBashes < BashCount)
            {
                remainingBashes = BashCount;
                OnBashRefill();
            }

        }

        void CheckStartBash()
        {

            if (State == PlayerState.Movement && bashCooldownTimer.Done && (remainingBashes > 0 || BashUnlimited) && Inputs.BashDown.Get(BashBuffer))
            {

                Inputs.BashDown.Clear();
                jumpHoldTimer.Stop(); // cancel previous jump

                // Consume bash token if not unlimited
                if (!BashUnlimited)
                {
                    --remainingBashes;
                }

                // Check if angle is within rounding range
                float angle = Inputs.BashAngle;
                float rounded = Mathf.Round(angle / 90f) * 90f;
                if (Mathf.Abs(Mathf.DeltaAngle(angle, rounded)) < BashAngleClamp / 2f)
                {
                    angle = rounded;
                }

                // Calculate bash directions
                angle *= Mathf.Deg2Rad;
                bashNormal = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                bashDir = Util.CalculateDirection(bashNormal.x);

                // Initialize timers
                bashTimer.Start();
                bashRefreshTimer.Start();
                boundWindowTimer.Start();

                OnBashStart();

                State = PlayerState.Bash;

            }

        }

        void UpdateBash()
        {

            if (bashTimer.Running)
            {

                velocity = BashSpeed * bashNormal;
                AnyJump();

                if (State == PlayerState.Movement)
                {
                    EndBash();
                }

            }
            else
            {

                State = PlayerState.Movement;
                EndBash();

            }

            bashCooldownTimer.Start();
            boundWindowTimer.Start();

        }

        void EndBash()
        {

            velocity = Vector2.MoveTowards(velocity, Vector2.zero, BashEndDecrease);
            OnBashEnd();

        }

        #endregion

    }

}
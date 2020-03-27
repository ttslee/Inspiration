using System;
using UnityEngine;

namespace Engarde_Bryan {

	[Serializable]
	public class ScreenShakeTool {

		public static float GlobalScreenshakeFactor => 1f;

		public Vector2 Offset { get; private set; }

		public SimpleTimer time = new SimpleTimer(1);
		public AnimationCurve dampingCurve = AnimationCurve.Linear(0, 1, 1, 0);

		[Tooltip("'a' term in ax^n + b.")]
		public float durationScale = 0.3f;
		[Tooltip("'n' term in ax^n + b.")]
		public float scalePower = 1f;
		[Tooltip("'b' term in ax^n + b.")]
		public float baseDuration = 0.1f;

		[Tooltip("The number of sine cycles to perform.")]
		public float repeats = 1;
		[Tooltip("Speed to move back toward origin on Stop().")]
		public float dampAmount = 3f;
		public Vector2 direction;
		public float magnitude;
		public bool stopFlag;

		public void Start(float magnitude, Vector2 direction) {
			if (GlobalScreenshakeFactor == 0) return;

			this.magnitude = GlobalScreenshakeFactor * magnitude;
			this.direction = direction;

			stopFlag = false;
			time.Duration = durationScale * Mathf.Pow(this.magnitude, scalePower) + baseDuration;
			//Debug.Log(time.Duration);
			time.Start();
		}

		public void Update() {
			if (!stopFlag && GlobalScreenshakeFactor > 0) {

				time.Update();

				if (time.Running && repeats > 0) {
					Offset = magnitude * Mathf.Sin(time.Progress * 2 * Mathf.PI * repeats) * dampingCurve.Evaluate(time.Progress) * direction;
				} else {
					Offset = Vector2.zero;
				}

			} else {
				Offset = Vector2.MoveTowards(Offset, Vector2.zero, dampAmount * Time.deltaTime);
			}
		}

		public void Stop() {
			stopFlag = true;
		}

	}

}
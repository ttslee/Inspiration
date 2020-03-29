using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan {

	public class CameraController : MonoBehaviour {

		public static CameraController Instance { get; private set; }
		public static Camera MainCamera { get; private set; }

		[SerializeField] private Vector2 m_position;
		/// <summary>
		/// The raw position of the camera before screenshake is applied.
		/// <para><i>Use this property over transform.position</i></para>
		/// </summary>
		public Vector2 Position {
			get => m_position;
			set => m_position = value;
		}

		public Transform trackTarget { get; set; } = null;
		public float smoothtime = 0.02f;
		private Vector2 refVel;
		public ScreenShakeTool screenshake;

		private void Awake() {
			MainCamera = GetComponent<Camera>();
			Instance = this;
		}

		private void LateUpdate() {
            if(trackTarget != null) {
                Vector2 target = trackTarget.position;
                Position = Vector2.SmoothDamp(Position, target, ref refVel, smoothtime);

                screenshake.Update();
                Vector2 pfinal = Position + screenshake.Offset;
                transform.position = new Vector3(pfinal.x, pfinal.y, transform.position.z);
            }
        }

		/// <summary>
		/// Request the camera begin a screenshake with given magnitude and initial direction.
		/// </summary>
		public void RequestScreenShake(float magnitude, Vector2 direction) {
			screenshake.Start(magnitude, (direction.sqrMagnitude == 0 ? Vector2.down : direction).normalized);
		}

		/// <summary>
		/// Request the camera begin a screenshake with given magnitude and a random initial direction.
		/// </summary>
		public void RequestScreenShake(float magnitude) => RequestScreenShake(magnitude, UnityEngine.Random.insideUnitCircle);

	}

}
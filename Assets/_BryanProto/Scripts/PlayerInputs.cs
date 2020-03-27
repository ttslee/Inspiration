using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	public class InputBuffer {

		private float time { get; set; }

		public InputBuffer() {
			Clear();
		}

		public void Set() => time = Time.time;
		public bool Get(float maxBuffer) => Time.time - time < maxBuffer;
		public void Clear() => time = float.MinValue;

	}

	/// <summary>
	/// Inputs to the PlayerController system.
	/// </summary>
	public class InputCommands {

		public float Horizontal { get; set; }

		public InputBuffer JumpDown { get; set; }
		public bool JumpHold { get; set; }

		public InputBuffer BashDown { get; set; }
		/// <summary>
		/// Angle in radians. 0 is to the right.
		/// </summary>
		public float BashAngle { get; set; }

		public InputCommands() {
			JumpDown = new InputBuffer();
			BashDown = new InputBuffer();
		}

	}

}
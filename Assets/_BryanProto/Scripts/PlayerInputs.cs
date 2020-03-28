using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	/// <summary>
	/// Buffer an input down event.
	/// </summary>
	public class InputBuffer {

		private float Timestamp { get; set; }

		/// <summary>
		/// Create a new buffer starting in the cleared state.
		/// </summary>
		public InputBuffer() {
			Clear();
		}

		/// <summary>
		/// Set the buffer to the current time.
		/// </summary>
		public void Set() => Timestamp = Time.time;

		/// <summary>
		/// Check if buffer is in given window. Does not consume.
		/// </summary>
		public bool Get(float maxBuffer) => Time.time - Timestamp < maxBuffer;

		/// <summary>
		/// Consume the buffer.
		/// </summary>
		public void Clear() => Timestamp = float.MinValue;

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
		/// Angle in degrees. 0 is to the right.
		/// </summary>
		public float BashAngle { get; set; }

		public InputCommands() {
			JumpDown = new InputBuffer();
			BashDown = new InputBuffer();
		}

	}

}
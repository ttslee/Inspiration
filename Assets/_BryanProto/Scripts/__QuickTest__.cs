using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan {

	public class __QuickTest__ : MonoBehaviour {

		public float mag = 0.5f;

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Y)) {
				CameraController.Instance.RequestScreenShake(mag);
			}
		}

	}

}

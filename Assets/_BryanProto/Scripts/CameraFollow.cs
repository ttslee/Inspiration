using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde.Bryan {

	public class CameraFollow : MonoBehaviour {

		public Transform target;

		private void LateUpdate() {
			transform.position = target.position;
		}

	}

}
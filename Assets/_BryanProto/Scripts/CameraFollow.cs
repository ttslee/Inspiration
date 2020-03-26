using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan {

	public class CameraFollow : MonoBehaviour {

		public Transform target;
		private float zoffset;

		private void Awake() {
			zoffset = transform.position.z;
		}

		private void LateUpdate() {
			Vector3 pos = target.position;
			pos.z = zoffset;
			transform.position = pos;
		}

	}

}
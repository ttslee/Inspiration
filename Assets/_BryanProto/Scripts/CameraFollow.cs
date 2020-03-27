using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan {

	public class CameraFollow : MonoBehaviour {

		public Transform target;
		private float zoffset;

		public float smoothtime = 0.02f;
		private Vector3 refVel;

		private void Awake() {
			zoffset = transform.position.z;
		}

		private void LateUpdate() {
			Vector3 pos = target.position;
			pos.z = zoffset;
			transform.position = Vector3.SmoothDamp(transform.position, pos, ref refVel, smoothtime);
		}

	}

}
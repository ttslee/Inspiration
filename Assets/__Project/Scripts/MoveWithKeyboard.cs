using UnityEngine;

namespace Engarde {

	public class MoveWithKeyboard : MonoBehaviour {

		public float speed = 8f;
		public float nyoomMultiplier = 6f;

		private void Update() {
			float s = Input.GetKey(KeyCode.LeftShift) ? speed * nyoomMultiplier : speed;
			transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal") * s * Time.deltaTime, Input.GetAxisRaw("Vertical") * s * Time.deltaTime));
		}

	}

}
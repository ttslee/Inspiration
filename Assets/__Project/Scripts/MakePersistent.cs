using UnityEngine;

namespace Engarde {

	public sealed class MakePersistent : MonoBehaviour {

		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}

	}

}
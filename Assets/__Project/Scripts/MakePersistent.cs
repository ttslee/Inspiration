using UnityEngine;

namespace Engarde_Bryan {

	public sealed class MakePersistent : MonoBehaviour {

		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}

	}

}
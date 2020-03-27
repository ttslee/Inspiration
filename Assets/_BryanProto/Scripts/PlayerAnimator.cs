using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Bryan.Player {

	public class PlayerAnimator : MonoBehaviour {

		public Sprite idle;
		public Sprite run;

		public Color[] colors;

		SpriteRenderer sr;
		Rigidbody2D body;
		PlayerController controller;

		ParticleSystem particles;

		private void Awake() {
			body = GetComponent<Rigidbody2D>();
			sr = GetComponent<SpriteRenderer>();
			controller = GetComponent<PlayerController>();
			particles = GetComponentInChildren<ParticleSystem>();
		}


		private void Update() {

			bool running = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.2f;

			if (running) {
				sr.sprite = run;
			} else {
				sr.sprite = idle;
			}

			if (running) {
				sr.flipX = Input.GetAxisRaw("Horizontal") < 0f;
			}

			sr.color = colors[Mathf.Clamp(controller.remainingBashes, 0, colors.Length)];

			particles.transform.localPosition = Vector2.ClampMagnitude(-body.velocity, 1f);

		}

	}

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Teo.Player
{

    public class PlayerAnimator : MonoBehaviour
    {


        [Header("Constants")]

        public Sprite idle;
        public Sprite run;

        public Color[] colors;
        private Color CurrentColor => colors[Mathf.Clamp(controller.remainingBashes, 0, colors.Length - 1)];

        [Space]
        public float TrailSpacing = 1f;
        public float TrailTimeExtend = 0.6f;
        public GameObject TrailPrefab;
        public Transform trailContainer;
        public float TrailAlpha = 0.5f;
        public float TrailDestroyDelay = 0.2f;
        public float TrailDestroySpacing = 0.02f;


        SpriteRenderer sr;
        Rigidbody2D body;
        PlayerController controller;

        ParticleSystem particles;
        TrailRenderer trailLine;


        [Header("Variables")]

        public bool trail;


        private readonly List<GameObject> trailObjects = new List<GameObject>();


        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            controller = GetComponent<PlayerController>();

            particles = GetComponentInChildren<ParticleSystem>();
            trailLine = GetComponentInChildren<TrailRenderer>();

            trailLine.Clear();
            trailLine.emitting = false;
        }


        internal void Animate()
        {

            bool running = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.2f;

            if (running)
            {
                sr.sprite = run;
            }
            else
            {
                sr.sprite = idle;
            }

            if (running)
            {
                sr.flipX = Input.GetAxisRaw("Horizontal") < 0f;
            }

            sr.color = CurrentColor;

            particles.transform.localPosition = Vector2.ClampMagnitude(-body.velocity, 1f);

        }

        internal void AnimateFixed()
        {


        }

        #region Trail

        Queue<Coroutine> curCR = new Queue<Coroutine>();

        public void BeginTrail()
        {
            curCR.Enqueue(StartCoroutine(C_DrawTrail()));
            trailLine.Clear();
            trailLine.emitting = true;
        }

        public void EndTrail()
        {
            StartCoroutine(C_DeleteTrail(curCR.Dequeue()));
        }

        IEnumerator C_DrawTrail()
        {

            Vector2 startpos = controller.Position;
            int next = 0;
            Color color = Util.SetAlpha(CurrentColor, TrailAlpha);

            while (true)
            {

                var ob = Instantiate(TrailPrefab, transform.position, TrailPrefab.transform.rotation, trailContainer);
                ob.GetComponent<TrailAnimate>().dir = -controller.bashNormal;
                var osr = ob.GetComponent<SpriteRenderer>().color = color;
                trailObjects.Add(ob);

                ++next;
                yield return new WaitUntil(
                    () => Vector2.Distance(startpos, controller.Position) > TrailSpacing * next);

            }

        }

        IEnumerator C_DeleteTrail(Coroutine targetCR)
        {

            yield return new WaitForSeconds(TrailTimeExtend);
            StopCoroutine(targetCR);
            trailLine.emitting = false;

            while (trailObjects.Count > 0)
            {
                Destroy(trailObjects[0], TrailDestroyDelay);
                trailObjects.RemoveAt(0);
                yield return new WaitForSeconds(TrailDestroySpacing);
            }

        }

        #endregion

    }

}
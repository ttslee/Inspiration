using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Teo.Player
{

    public class TrailAnimate : MonoBehaviour
    {

        public float speed = 0.4f;
        public Vector3 dir;

        private void Update()
        {
            transform.Translate(dir * speed * Time.deltaTime);
        }

    }

}
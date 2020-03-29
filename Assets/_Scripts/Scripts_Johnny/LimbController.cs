﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Johnny.Player
{
    public class LimbController : MonoBehaviour
    {
        private List<Limb> limbs = new List<Limb>(); //list of all limbs from child to body
        public Transform root;

        //Arm functions
        public bool useMousePos = false;
        public bool getRoots = false;


        public Vector2 targetPos; //pos we want to rotate to
        private bool dirFlip = false;
        private bool sin = false;
        private int force = 10; //controls how strong/fast a limb moves

        void Start()
        {
            if (!getRoots)
                sin = true;
            HingeJoint2D hinge = GetComponent<HingeJoint2D>();
            while (hinge != null && hinge.transform != root)
            {
                float dist = transform.position.x - root.position.x;
                if (dist >= 0f)
                    dirFlip = false;
                else
                    dirFlip = true;
                limbs.Add(new Limb(hinge.attachedRigidbody, dirFlip, sin));
                if (getRoots)
                    hinge = hinge.connectedBody.GetComponent<HingeJoint2D>();
                else
                    hinge = null;
            }
            if (useMousePos == true)
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void FixedUpdate()
        {
            if (useMousePos == true)
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var limb in limbs)
                limb.RotateTowards(targetPos + (Vector2)root.position, force);
        }

        private void OnDrawGizmos()
        {
            foreach(var limb in limbs)
                limb.OnDrawGizmos();

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPos + (Vector2)root.position, .1f);
        }

    }
}
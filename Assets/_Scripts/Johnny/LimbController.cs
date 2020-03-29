using System.Collections;
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
        public bool dirFlip = false;
        public bool getRoots = false;

        public Vector2 targetPos; //pos we want to rotate to
        private bool sin = false;
        public int force = 10; //controls how strong/fast a limb moves

        void Start()
        {
            if (!getRoots)
                sin = true;
            HingeJoint2D hinge = GetComponent<HingeJoint2D>();
            while (hinge != null && hinge.transform != root)
            {
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
            foreach (var limb in limbs)
            {
                if (useMousePos == false)
                    limb.RotateTowards(targetPos + (Vector2)root.position, force);
                else
                    limb.RotateTowards(Camera.main.ScreenToWorldPoint(Input.mousePosition), force);
            }
        }

        private void OnDrawGizmos()
        {
            foreach(var limb in limbs)
                limb.OnDrawGizmos();

            Gizmos.color = Color.red;
            if (useMousePos == false)
                Gizmos.DrawSphere(targetPos + (Vector2)root.position, .1f);
            else
                Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), .1f);
        }

    }
}
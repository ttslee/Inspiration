using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Johnny.Player
{
    public class LimbController : MonoBehaviour
    {
        private List<Limb> limbs = new List<Limb>();
        public Transform root;
        public Vector2 targetPos;
        private bool dirFlip = false;
        private int force = 1;

        void Start()
        {
            HingeJoint2D hinge = GetComponent<HingeJoint2D>();
            while (hinge != null && hinge.transform != root)
            {
                float dist = transform.position.x - root.position.x;
                if (dist >= 0f)
                    dirFlip = false;
                else
                    dirFlip = true;
                limbs.Add(new Limb(hinge.attachedRigidbody, dirFlip));
                hinge = hinge.connectedBody.GetComponent<HingeJoint2D>();
            }
        }

        private void Update()
        {
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
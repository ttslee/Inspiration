using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engard_Johnny
{
    public class Limb //Each individual Limb
    {
        private Rigidbody2D rigidBody;
        private Transform root;

        Vector2 avgDirection;
        Vector2 avgWorldPos;
        private float ratio = 0.9f; //Limb weight
        private float wratio = .95f;

        private Vector2 offset;
        private Vector2 relativeOffset => offset.x/2 * root.right + offset.y/2 * root.up;

        private bool flip = false;

        public Limb(Rigidbody2D rb, bool flipped)
        {
            this.rigidBody = rb;
            this.root = rb.transform;
            this.flip = flipped;

            offset = root.GetComponent<HingeJoint2D>().anchor;

        }

        public void RotateTowards(Vector2 worldPos, float force)
        {
            //Limb weight adjusting
            float dif = (worldPos - avgWorldPos).magnitude;
            if (dif > 1)
                dif = 1;
            dif = 1 - dif;
            avgWorldPos = worldPos * (1 - wratio) + avgWorldPos * wratio;
            ratio += (1 - ratio) * dif * .95f;

            //Angle to move
            Vector2 direction = worldPos - ((Vector2)root.position + relativeOffset);
            if (flip)
                direction *= -1f;
            avgDirection = direction * (1 - ratio) + avgDirection * ratio;
            float angle = AngleFromDirection(avgDirection);

            angle = Mathf.DeltaAngle(root.eulerAngles.z, angle);
            float x = angle > 0 ? 1 : -1;
            angle = Mathf.Abs(angle * .1f);
            if (angle > 2)
                angle = 2;
            angle *= .5f;
            angle *= 1 + angle;

            rigidBody.angularVelocity *= angle * .5f;
            rigidBody.AddTorque(angle * force * x);
        }

        private float AngleFromDirection(Vector2 dir)
        {
            dir = dir.normalized;
            float angle = Mathf.Acos(dir.x) * Mathf.Rad2Deg;
            return dir.y > 0 ? angle : 360 - angle;
        }

        public void OnDrawGizmos()
        {
            Vector2 position = root.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position + relativeOffset, .1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position - relativeOffset, .1f);
        }
    }
}
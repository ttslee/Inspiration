using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Johnny.Player
{
    public class GroundCheck : MonoBehaviour
    {
        public LayerMask groundLayer;
        private CapsuleCollider2D capsuleCollider;

        void Start()
        {
            capsuleCollider = transform.GetComponent<CapsuleCollider2D>();
        }

        public bool IsGrounded()
        {
            float extraRange = 0.2f;
            RaycastHit2D rayCast = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, capsuleCollider.bounds.extents.y + extraRange, groundLayer);
            Color rayColor;
            if (rayCast.collider != null)
            {
                rayColor = Color.green;
            }
            else
                rayColor = Color.red;
            Debug.DrawRay(capsuleCollider.bounds.center, Vector2.down * (capsuleCollider.bounds.extents.y + extraRange), rayColor);
            return rayCast.collider != null;
        }

    }


}
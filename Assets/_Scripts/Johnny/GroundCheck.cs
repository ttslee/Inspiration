using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engarde_Johnny.Player
{
    public class GroundCheck : MonoBehaviour
    {
        public LayerMask groundLayer;
        public CapsuleCollider2D leftLegCollider;
        public CapsuleCollider2D rightLegCollider;

        public bool IsGrounded()
        {
            float extraRange = .5f;
            RaycastHit2D rayCast1 = Physics2D.Raycast(leftLegCollider.bounds.center, Vector2.down, leftLegCollider.bounds.extents.y + extraRange, groundLayer);
            RaycastHit2D rayCast2 = Physics2D.Raycast(rightLegCollider.bounds.center, Vector2.down, rightLegCollider.bounds.extents.y + extraRange, groundLayer);
            Color rayColor;
            if (rayCast1.collider != null || rayCast2.collider != null)
            {
                rayColor = Color.green;
            }
            else
                rayColor = Color.red;
            Debug.DrawRay(leftLegCollider.bounds.center, Vector2.down * (leftLegCollider.bounds.extents.y + extraRange), rayColor);
            Debug.DrawRay(rightLegCollider.bounds.center, Vector2.down * (rightLegCollider.bounds.extents.y + extraRange), rayColor);
            return rayCast1.collider != null || rayCast2.collider != null;
        }

    }


}
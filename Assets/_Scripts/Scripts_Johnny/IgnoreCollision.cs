using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engard_Johnny
{
    public class IgnoreCollision : MonoBehaviour
    {
        //This script is mainly used to prevent Player limbs from touching eachother
        void Start()
        {
            var colliders = GetComponentsInChildren<Collider2D>(); //List of all Colliders under Parent
            for (int i = 0; i < colliders.Length; i++)
            {
                for (int j = i + 1; j < colliders.Length; j++)
                {
                    Physics2D.IgnoreCollision(colliders[i], colliders[j % colliders.Length]);
                }
            }
        }
    }
}
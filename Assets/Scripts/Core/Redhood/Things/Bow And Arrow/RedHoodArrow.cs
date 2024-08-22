using ChenChen_Core.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChenChen_Core
{
    public class RedHoodArrow : MonoBehaviour
    {
        private Vector3 origin;

        [Header("²ÎÊý")]
        public float Speed = 1f;
        public float MaxDistance = 10f;

        public SimpleComponentPool<RedHoodArrow> Pool;
        public Vector3 Dir = Vector3.zero;
        public void SetStartPosition(Vector3 startPosition)
        {
            origin = startPosition;
            transform.position = startPosition;
        }

        private void Update()
        {
            transform.position += Dir * Speed;

            if (Vector3.Distance(transform.position, origin) > MaxDistance)
            {
                Pool.Release(this);
            }
        }
    }
}
using ChenChen_Core.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class SamuraiDart : MonoBehaviour
    {
        private Vector3 origin;

        [Header("²ÎÊý")]
        public float Speed = 1f;
        public float MaxDistance = 10f;

        public SimpleComponentPool<SamuraiDart> Pool;
        public Vector3 Dir = Vector3.zero;
        public void SetStartPosition(Vector3 startPosition)
        {
            origin = startPosition;
            transform.position = origin;
        }

        public Transform body;

        private void Update()
        {
            if (Dir.x < 0)
            {
                body.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                body.localScale = Vector3.one;
            }
            transform.position += new Vector3(Dir.x, 0, 0) * Speed;

            if (Vector3.Distance(transform.position, origin) > MaxDistance)
            {
                Pool.Release(this);
            }
        }
    }
}
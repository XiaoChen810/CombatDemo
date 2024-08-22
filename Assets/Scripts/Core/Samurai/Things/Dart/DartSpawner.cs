using ChenChen_Core.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class DartSpawner : MonoBehaviour
    {
        private static readonly string s_DartPrefabPath = "Items/Dart/Dart";
        private GameObject DartPrefab;

        private SimpleComponentPool<SamuraiDart> dartPool;

        private void Start()
        {
            dartPool = new SimpleComponentPool<SamuraiDart>(s_DartPrefabPath, ActionGet, ActionRelease);
        }

        private void ActionGet(SamuraiDart dart)
        {
            dart.Pool = dartPool;
            dart.SetStartPosition(transform.position);
        }

        private void ActionRelease(SamuraiDart dart)
        {
            dart.Dir = Vector3.zero;
        }

        public void Shoot(Vector3 dir)
        {
            SamuraiDart newDart = dartPool.Get();
            if (newDart != null)
            {
                newDart.Dir = dir;
            }
        }
    }
}
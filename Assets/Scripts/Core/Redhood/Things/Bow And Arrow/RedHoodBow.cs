using ChenChen_Core.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class RedHoodBow : MonoBehaviour
    {
        private static readonly string s_ArrowPrefabPath = "Items/Arrow/Arrow";

        private SimpleComponentPool<RedHoodArrow> arrowPool;
        private RedHood redHood = null;
        private RedHoodArrow currentArrow = null;
        private bool faceRight = false;

        private void Start()
        {
            redHood = GetComponentInParent<RedHood>();
            if (redHood ==  null)
            {
                Debug.LogWarning("×é¼þÎª¿Õ");
            }

            arrowPool = new SimpleComponentPool<RedHoodArrow>(s_ArrowPrefabPath, ActionGet, ActionRelease);
        }

        private void ActionGet(RedHoodArrow arrow)
        {
            arrow.Pool = arrowPool;
            arrow.SetStartPosition(transform.position);
        }

        private void ActionRelease(RedHoodArrow arrow)
        {
            arrow.Dir = Vector3.zero;
        }

        public void DrawBow()
        {
            if (currentArrow == null && redHood != null)
            {
                currentArrow = arrowPool.Get();

                if (redHood.Facing > 0)
                {
                    faceRight = true;
                    currentArrow.transform.localScale = Vector3.one;
                }
                else
                {
                    faceRight = false;
                    currentArrow.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        public void LooseBow()
        {
            if (currentArrow != null)
            {
                Vector3 force = faceRight ? Vector3.right : Vector3.left;
                currentArrow.Dir = force;
                currentArrow = null;
            }
        }

        public void DestroyArrow()
        {
            if(currentArrow != null)
            {
                arrowPool.Release(currentArrow);
                currentArrow = null;
            }
        }
    }
}

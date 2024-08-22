using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class SamuraiConditional : Conditional
    {
        protected Rigidbody rb;
        protected Animator anim;
        protected Samurai samurai;
        protected RedHood redHood;

        public override void OnAwake()
        {
            anim = transform.Find("Body").GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Animator 组件为空");
            }

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody 组件为空");
            }

            samurai = GetComponent<Samurai>();
            if(samurai == null)
            {
                Debug.LogError("Samurai 组件为空");
            }

            redHood = GameObject.FindAnyObjectByType<RedHood>();
            if (redHood == null)
            {
                Debug.LogError("RedHood 目标为空");
            }
        }
    }
}
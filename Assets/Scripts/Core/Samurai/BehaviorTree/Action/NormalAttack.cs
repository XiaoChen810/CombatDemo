using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class NormalAttack : SamuraiAction
    {
        public string AnimationName = "";

        public float AnimationTime;

        private float timer = 0;

        public override void OnStart()
        {
            anim.SetTrigger(AnimationName);
            timer = AnimationTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                return TaskStatus.Running;
            }

            if (samurai.IsBroken)
            {
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }
    }
}
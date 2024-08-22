using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class StepBack : SamuraiAction
    {
        public string AnimationName;
        public float AnimationTime;

        public float MoveSpeed = 1;

        private float timer;

        public override void OnStart()
        {
            timer = AnimationTime;
            samurai.PlayAnimation(AnimationName);
        }

        public override TaskStatus OnUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                samurai.MoveOnly(MoveSpeed * -samurai.Facing);
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}
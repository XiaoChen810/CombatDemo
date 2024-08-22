using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class DartAttack : SamuraiAction
    {
        public string AnimationName = "";

        public float AnimationTime;

        public int number;

        private float timer = 0;
        private int dartsShot = 0;

        public override void OnStart()
        {
            dartsShot = 0;
        }

        public override TaskStatus OnUpdate()
        {
            if (dartsShot < number)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    anim.SetTrigger(AnimationName);
                    samurai.ShootDart(samurai.Body.transform.localScale);
                    dartsShot++;
                    timer = AnimationTime; // 重置计时器，准备下一次发射
                }

                return TaskStatus.Running; // 继续运行任务，直到发射完所有飞镖
            }

            return TaskStatus.Success; // 所有飞镖发射完毕，任务成功完成
        }
    }
}

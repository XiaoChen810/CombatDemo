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
                    timer = AnimationTime; // ���ü�ʱ����׼����һ�η���
                }

                return TaskStatus.Running; // ������������ֱ�����������з���
            }

            return TaskStatus.Success; // ���з��ڷ�����ϣ�����ɹ����
        }
    }
}

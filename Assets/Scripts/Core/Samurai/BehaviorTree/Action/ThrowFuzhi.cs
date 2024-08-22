using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class ThrowFuzhi : SamuraiAction
    {
        private static readonly string s_FuzhiPrefabPath = "Items/Fuzhi/Fuzhi";

        public string AnimationName;
        public float AnimationTime;

        public float FuzhiSpeedMultiplier;
        public int FuzhiNumber;

        public float FuzhiSpeedParabola_Min = 1;    //施加一个抛物线的力
        public float FuzhiSpeedParabola_Max = 15;    //施加一个抛物线的力

        private float timer;
        private GameObject fuzhiPrefab;

        public override void OnAwake()
        {
            base.OnAwake();
            fuzhiPrefab = Resources.Load<GameObject>(s_FuzhiPrefabPath);
            if (fuzhiPrefab == null)
            {
                Debug.LogError("无法从该位置加载符纸的预制件: " + s_FuzhiPrefabPath);
            }
        }

        public override void OnStart()
        {
            timer = AnimationTime;
            samurai.PlayAnimation(AnimationName);           
        }

        public override TaskStatus OnUpdate()
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                return TaskStatus.Running;
            }

            // 向前方扔出数张符纸
            for(int i = 0; i < FuzhiNumber; i++)
            {
                float speedMultiplier = (1 + FuzhiSpeedMultiplier) * i;
                Fuzhi fuzhi = GameObject.Instantiate(fuzhiPrefab, samurai.dartSpawner.transform.position, Quaternion.identity).GetComponent<Fuzhi>();
                if (fuzhi != null)
                {
                    float originSpeed = fuzhi.Speed;
                    originSpeed *= -samurai.Facing;
                    fuzhi.Speed = originSpeed * (speedMultiplier + 1);

                    float parabola = Random.Range(FuzhiSpeedParabola_Min, FuzhiSpeedParabola_Max);
                    fuzhi.UpForce = parabola;
                }
            }

            return TaskStatus.Success;
        }
    }
}
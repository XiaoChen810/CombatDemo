using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class FarAwayFromPlayer : SamuraiAction
    {
        public float appealPositionOffest = 0;  // 出现的位置，基于玩家位置

        public string AnimationName;
        
        public float AnimationDuration; // 动画持续时间
        public float ActionDuration;    // 行动总时间

        private float timer = 0;

        public override void OnStart()
        {
            if (!samurai.IsOnGround) return;

            StartCoroutine(EffectCo());
            timer = ActionDuration;
        }

        public override TaskStatus OnUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            samurai.Body.SetActive(true);
        }

        IEnumerator EffectCo()
        {
            anim.SetTrigger(AnimationName);
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
 
            yield return new WaitForSeconds(AnimationDuration);

            samurai.Body.SetActive(false);
            Vector3 newPosition = new Vector3(GetXPositionOffset(), samurai.transform.position.y, samurai.transform.position.z);
            if (GameManager.Instance.CheckPosition(newPosition))
            {
                samurai.transform.position = newPosition;
            }
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        private float GetXPositionOffset()
        {
            float dir = appealPositionOffest > 0 ? 1 : -1;
            dir = dir * redHood.Facing;
            return redHood.transform.position.x + Mathf.Abs(appealPositionOffest) * dir;
        }
    }
}
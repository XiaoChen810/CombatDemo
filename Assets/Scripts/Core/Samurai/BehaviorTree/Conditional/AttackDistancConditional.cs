using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class AttackDistancConditional : SamuraiConditional
    {
        private float AttackRange2 = 4f;

        public override TaskStatus OnUpdate()
        {
            float distanceX = redHood.transform.position.x - samurai.transform.position.x;
            float distanceZ = redHood.transform.position.z - samurai.transform.position.z;
            float distance2 = distanceX * distanceX + distanceZ * distanceZ;

            if (distance2 <= AttackRange2)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
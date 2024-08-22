using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class ChasePlayer : SamuraiAction
    {
        public float Speed = 5f;

        public float DistanceOfEnd = 2f;

        public override TaskStatus OnUpdate()
        {
            float distanceX = redHood.transform.position.x - samurai.transform.position.x;
            float distanceZ = redHood.transform.position.z - samurai.transform.position.z;
            float distance2 = distanceX * distanceX + distanceZ * distanceZ;

            if(distance2 <= DistanceOfEnd * DistanceOfEnd)
            {
                return TaskStatus.Success;
            }

            if (!samurai.IsBroken && !samurai.IsDefend)
            {
                samurai.MoveTarget(redHood.transform.position, Speed);
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            samurai.StopMove();
        }
    }
}
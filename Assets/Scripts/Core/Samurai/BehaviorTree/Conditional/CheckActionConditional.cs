using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class CheckActionConditional : SamuraiConditional
    {
        public override TaskStatus OnUpdate()
        {
            if(!samurai.CanAction)
            {
                return TaskStatus.Failure;
            }
            return TaskStatus.Success;
        }
    }
}
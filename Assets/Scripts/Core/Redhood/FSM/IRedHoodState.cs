using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public enum RHStateType
    {
        Idle,

        Jump, Fall,

        LightHit, HeavyHit, BowHit,

        Slide,

        DodgeRight, DodgeLeft,

        SpecialSkill,

        Hurt
    }

    public interface IRedHoodState
    {
        float MaxDuration { get; }
        RHStateType StateType { get; }
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}


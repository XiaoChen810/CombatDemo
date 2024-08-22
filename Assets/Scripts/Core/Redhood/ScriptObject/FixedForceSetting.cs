using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    [CreateAssetMenu(fileName = "FixedForceSettingDefault", menuName = "修复力设置")]
    public class FixedForceSetting : ScriptableObject
    {
        public List<FixedForce> ForceList;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    [CreateAssetMenu(fileName = "Bullet Setting Default", menuName = "子弹效果")]
    public class BulletSetting : ScriptableObject
    {
        public float speed = 10f;
        public float initialMoveDuration = 0;
        public float lerpArgument = 0.5f;

        public float angleStart = 0f;
        public float angleEnd = 60f;

        [Header("使用随机暂停")]
        public bool useRamdomStop = false;
        [Range(0f, 1f)] public float ramdomStopStrength = 0.1f;

        public int num = 10;
        public int count = 1;
    }
}
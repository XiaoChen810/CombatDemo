using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class PlayerAttackBox : MonoBehaviour
    {
        public GameObject Player;

        [Header("打击强度，敌人击退数值")]
        public float HitStrength;

        [Header("使用相机震动")]
        public bool UseCameraShake = true;
        public float CameraShakeDuration = 0.5f;
        public float CameraShakeStrenght = 0.03f;

        [Header("使用顿感")]
        public bool UseSudden = false;
        public float SuddenDuration = 1;

        private void OnEnable()
        {
            tag = "PlayerAttackBox";
            gameObject.layer = 9;
        }

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
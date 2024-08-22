using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class PlayerAttackBox : MonoBehaviour
    {
        public GameObject Player;

        [Header("���ǿ�ȣ����˻�����ֵ")]
        public float HitStrength;

        [Header("ʹ�������")]
        public bool UseCameraShake = true;
        public float CameraShakeDuration = 0.5f;
        public float CameraShakeStrenght = 0.03f;

        [Header("ʹ�öٸ�")]
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
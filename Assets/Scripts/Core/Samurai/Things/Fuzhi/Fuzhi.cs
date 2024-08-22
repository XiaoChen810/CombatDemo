using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class Fuzhi : MonoBehaviour
    {
        private static readonly string s_BoomEffectPrefabPath = "Items/Fuzhi/ExplosionEffect";

        private Rigidbody rb;
        private GameObject boomEffectPrefab;

        public float Speed;
        public float UpForce;
        public float BoomOffset = 1;

        public LayerMask TriggerLager;



        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(0, UpForce, 0), ForceMode.VelocityChange);
            boomEffectPrefab = Resources.Load<GameObject>(s_BoomEffectPrefabPath);
            if(boomEffectPrefab == null)
            {
                Debug.LogError("无法加载爆炸特效预制件检查位置: " + s_BoomEffectPrefabPath);
            }
        }

        private void Update()
        {
            rb.velocity = new Vector3(Speed, rb.velocity.y, rb.velocity.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(((1 << other.gameObject.layer) & TriggerLager) != 0)
            {
                GameObject.Instantiate(boomEffectPrefab, transform.position + Vector3.up * BoomOffset, Quaternion.identity);
                AudioManager.Instance.PlaySoundEffect("爆炸");
                Destroy(gameObject);
            }
        }
    }
}
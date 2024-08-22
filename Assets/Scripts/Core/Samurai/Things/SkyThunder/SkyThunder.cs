using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class SkyThunder : MonoBehaviour
    {
        private static readonly string s_LazerPrefabPath = "Items/SkyThunder/Lazer";

        private GameObject LazerPrefab;
        private ParticleSystem me;

        private void Start()
        {
            me = GetComponent<ParticleSystem>();
            LazerPrefab = Resources.Load<GameObject>(s_LazerPrefabPath);
            if (LazerPrefab == null)
            {
                Debug.LogError("无法从该位置加载激光束的预制件: " +  s_LazerPrefabPath);
            }
        }

        private void Update()
        {
            if (me.isStopped)
            {
                Instantiate(LazerPrefab, transform.position, Quaternion.identity);
                AudioManager.Instance.PlaySoundEffect("激光");
                Destroy(gameObject);
            }
        }
    }
}
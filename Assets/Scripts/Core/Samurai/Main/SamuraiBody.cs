using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class SamuraiBody : MonoBehaviour
    {
        private Samurai samurai;

        private void Start()
        {
            samurai = GetComponentInParent<Samurai>();

            if (samurai == null)
            {
                Debug.LogError("未能从父节点获取组件 Samurai");
            }
        }

        public void PlaySFXOneShot(string name)
        {
            AudioManager.Instance.PlaySoundEffect(name);
        }
    }
}
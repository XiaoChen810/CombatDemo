using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class RedHoodBody : MonoBehaviour
    {
        private RedHood redHood;

        void Start()
        {
            redHood = GetComponentInParent<RedHood>();

            if (redHood == null)
            {
                Debug.LogError("无法从父节点获取对应RedHood组件");
            }
        }

        // 因为动画机机制，需要代为转发
        public void AttackOver()
        {
            redHood.AttackOver();
        }

        public void PlaySFXOneShot(string name)
        {
            AudioManager.Instance.PlaySoundEffect(name);
        }
    }
}
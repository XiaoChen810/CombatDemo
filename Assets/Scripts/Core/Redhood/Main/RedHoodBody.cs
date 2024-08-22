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
                Debug.LogError("�޷��Ӹ��ڵ��ȡ��ӦRedHood���");
            }
        }

        // ��Ϊ���������ƣ���Ҫ��Ϊת��
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
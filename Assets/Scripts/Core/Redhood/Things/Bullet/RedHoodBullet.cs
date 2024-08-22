using ChenChen_Core.Pool;
using UnityEngine;

namespace ChenChen_Core
{
    public class RedHoodBullet : MonoBehaviour
    {
        public SimpleComponentPool<RedHoodBullet> pool;

        [Header("�ӵ�����")]
        public Transform target; // ׷�ٵ�Ŀ��
        public float speed = 10f; // �ӵ��ٶ�
        public float rotateSpeed = 200f; // �ӵ���ת�ٶ�
        public float initialMoveDuration = 0.5f; // ��ʼֱ���ƶ�ʱ��
        public float lerpArgument;  // ��ֵ����

        public bool useRandomStop = false;
        public float randomStopStrengh = 0.1f;

        private float maxTime = 10f;
        private float maxTimer = 10f;

        private void OnEnable()
        {
            maxTimer = maxTime;
        }

        void Update()
        {
            maxTimer -= Time.deltaTime;

            if (target == null)
            {
                Debug.LogError("Target is nulll");
                return;
            }

            if (maxTimer < 0)
            {
                pool.Release(this);
                return;
            }

            if (CalculateDestroyDistance(transform.position, target.position, 1f))
            {
                pool.Release(this);
                return;
            }

            float curSpeed = speed;

            if (useRandomStop && Random.value < randomStopStrengh)
            {
                curSpeed *= Random.value;
            }

            if (initialMoveDuration > 0)
            {
                // ��ʼֱ���ƶ�
                transform.Translate(transform.right * curSpeed * Time.deltaTime, 0);
                initialMoveDuration -= Time.deltaTime;
            }
            else
            {
                // ׷��Ŀ��
                Vector3 targetVector = target.position - transform.position;

                // ���ݾ�������ֵ����
                var t = lerpArgument / Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(target.position.x, target.position.y));

                // ƽ����ת�ӵ�����
                transform.right = Vector3.Slerp(transform.right, targetVector, t);

                // �ƶ��ӵ�
                transform.position += transform.right * curSpeed * Time.deltaTime;
            }
        }

        private bool CalculateDestroyDistance(Vector3 a, Vector3 b, float distance)
        {
            // ʹ��ƽ��������бȽϣ��Ա��ⲻ��Ҫ�Ŀ�ƽ������
            return (a - b).sqrMagnitude < distance * distance;
        }
    }
}

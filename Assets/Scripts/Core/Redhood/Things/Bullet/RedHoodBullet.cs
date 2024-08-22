using ChenChen_Core.Pool;
using UnityEngine;

namespace ChenChen_Core
{
    public class RedHoodBullet : MonoBehaviour
    {
        public SimpleComponentPool<RedHoodBullet> pool;

        [Header("子弹参数")]
        public Transform target; // 追踪的目标
        public float speed = 10f; // 子弹速度
        public float rotateSpeed = 200f; // 子弹旋转速度
        public float initialMoveDuration = 0.5f; // 初始直线移动时间
        public float lerpArgument;  // 插值参数

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
                // 初始直线移动
                transform.Translate(transform.right * curSpeed * Time.deltaTime, 0);
                initialMoveDuration -= Time.deltaTime;
            }
            else
            {
                // 追踪目标
                Vector3 targetVector = target.position - transform.position;

                // 根据距离计算插值参数
                var t = lerpArgument / Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(target.position.x, target.position.y));

                // 平滑旋转子弹朝向
                transform.right = Vector3.Slerp(transform.right, targetVector, t);

                // 移动子弹
                transform.position += transform.right * curSpeed * Time.deltaTime;
            }
        }

        private bool CalculateDestroyDistance(Vector3 a, Vector3 b, float distance)
        {
            // 使用平方距离进行比较，以避免不必要的开平方操作
            return (a - b).sqrMagnitude < distance * distance;
        }
    }
}

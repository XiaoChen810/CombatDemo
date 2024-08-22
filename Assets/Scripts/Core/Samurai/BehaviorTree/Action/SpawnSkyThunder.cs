using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class SpawnSkyThunder : SamuraiAction
    {
        private static readonly string s_SkyThunderPrefabPath = "Items/SkyThunder/SkyThunder";

        private GameObject SkyThunderPrefab;

        public string AnimationName;
        public float AnimationTime;

        public int SkyThunderNumber;
        public float SkyThuderHeight;
        public float SkyThunderSpacing;
        public Vector2 SkyThunderRange;

        private float timer;

        public override void OnAwake()
        {
            base.OnAwake();
            SkyThunderPrefab = Resources.Load<GameObject>(s_SkyThunderPrefabPath);
            if (SkyThunderPrefab == null)
            {
                Debug.LogError("无法从该位置加载SkyThunderPrefab: " + s_SkyThunderPrefabPath);
            }
        }

        public override void OnStart()
        {
            timer = AnimationTime;
            samurai.PlayAnimation(AnimationName);
        }

        public override TaskStatus OnUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                return TaskStatus.Running;
            }

            // 确保 SkyThunderPrefab 有效
            if (SkyThunderPrefab == null)
            {
                Debug.LogError("SkyThunderPrefab 为空，无法生成天雷");
                return TaskStatus.Failure;
            }

            // 固定的 Y 轴位置
            float yPosition = SkyThuderHeight;

            // 随机生成天雷位置的列表
            List<Vector3> spawnPositions = new List<Vector3>();

            int maxAttempts = 100; // 设置最大尝试次数
            int attempts = 0;

            while (spawnPositions.Count < SkyThunderNumber && attempts < maxAttempts)
            {
                // 生成随机位置
                float xPosition = Random.Range(SkyThunderRange.x, SkyThunderRange.y);

                // 确保天雷之间的间隔
                bool isValidPosition = true;
                foreach (var position in spawnPositions)
                {
                    if (Mathf.Abs(position.x - xPosition) < SkyThunderSpacing)
                    {
                        isValidPosition = false;
                        break;
                    }
                }

                if (isValidPosition)
                {
                    spawnPositions.Add(new Vector3(xPosition, yPosition, samurai.transform.position.z));
                }

                attempts++;
            }

            if (spawnPositions.Count < SkyThunderNumber)
            {
                Debug.LogWarning("无法生成足够的天雷，可能是由于间隔或范围设置不合适。");
            }

            // 生成天雷
            foreach (var position in spawnPositions)
            {
                GameObject.Instantiate(SkyThunderPrefab, position, Quaternion.identity);
            }

            return TaskStatus.Success;
        }


    }
}
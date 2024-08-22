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
                Debug.LogError("�޷��Ӹ�λ�ü���SkyThunderPrefab: " + s_SkyThunderPrefabPath);
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

            // ȷ�� SkyThunderPrefab ��Ч
            if (SkyThunderPrefab == null)
            {
                Debug.LogError("SkyThunderPrefab Ϊ�գ��޷���������");
                return TaskStatus.Failure;
            }

            // �̶��� Y ��λ��
            float yPosition = SkyThuderHeight;

            // �����������λ�õ��б�
            List<Vector3> spawnPositions = new List<Vector3>();

            int maxAttempts = 100; // ��������Դ���
            int attempts = 0;

            while (spawnPositions.Count < SkyThunderNumber && attempts < maxAttempts)
            {
                // �������λ��
                float xPosition = Random.Range(SkyThunderRange.x, SkyThunderRange.y);

                // ȷ������֮��ļ��
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
                Debug.LogWarning("�޷������㹻�����ף����������ڼ����Χ���ò����ʡ�");
            }

            // ��������
            foreach (var position in spawnPositions)
            {
                GameObject.Instantiate(SkyThunderPrefab, position, Quaternion.identity);
            }

            return TaskStatus.Success;
        }


    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBox : MonoBehaviour
{
    public GameObject parent;
    public ParticleSystem shootDownPrefab;

    [Header("����")]
    public float damage = 10;
    public float allowDodgeTime = 0.1f;
    public bool canShootDown;

    private void OnEnable()
    {
        tag = "EnemyAttackBox";
        gameObject.layer = 10;
    }

    public void ShoorDown()
    {
        if (canShootDown && parent != null)  // ��� null ���
        {
            Vector3 spawnPosition = parent.transform.position;
            Instantiate(shootDownPrefab, spawnPosition, Quaternion.identity);
            Destroy(parent);
        }
    }


    private void Start()
    {
        tag = "EnemyAttackBox";
        if (canShootDown && (parent == null || shootDownPrefab == null))
        {
            Debug.LogError("���δ����");
        }
    }

}

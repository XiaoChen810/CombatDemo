using UnityEngine;
using System.Collections;

public class AfterImageEffect : MonoBehaviour
{
    [Header("��Ӱ�������")]
    public SpriteRenderer bodySR;
    public GameObject afterImagePrefab; // ��ɫ��ӰԤ����

    [Header("��Ӱ����")]
    public float lifeTime = 0.5f; // ��Ӱ����ʱ��
    public float spawnInterval = 0.1f; // ��Ӱ���ɼ��

    private bool isGeneratingAfterImages = false;
    private Coroutine open;

    void Start()
    {
        StartCoroutine(GenerateAfterImages());
    }

    IEnumerator GenerateAfterImages()
    {
        while (true)
        {
            if (isGeneratingAfterImages)
            {
                // ���ɲ�Ӱ
                GameObject afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
                afterImage.transform.localScale = bodySR.gameObject.transform.localScale;
                afterImage.GetComponent<SpriteRenderer>().sprite = bodySR.sprite;

                // ���ٲ�Ӱ
                Destroy(afterImage, lifeTime);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OpenEffect(float duration)
    {
        if(open != null)
        {
            StopCoroutine(open);
        }
        open = StartCoroutine(OpenCo(duration));
    }

    IEnumerator OpenCo(float duration)
    {
        isGeneratingAfterImages = true;

        yield return new WaitForSeconds(duration);

        isGeneratingAfterImages = false;
    }
}

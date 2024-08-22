using UnityEngine;
using System.Collections;

public class AfterImageEffect : MonoBehaviour
{
    [Header("残影必须组件")]
    public SpriteRenderer bodySR;
    public GameObject afterImagePrefab; // 角色残影预制体

    [Header("残影参数")]
    public float lifeTime = 0.5f; // 残影持续时间
    public float spawnInterval = 0.1f; // 残影生成间隔

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
                // 生成残影
                GameObject afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
                afterImage.transform.localScale = bodySR.gameObject.transform.localScale;
                afterImage.GetComponent<SpriteRenderer>().sprite = bodySR.sprite;

                // 销毁残影
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

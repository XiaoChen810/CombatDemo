using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using ChenChen_Core;

public class AttackManager : MonoBehaviour
{
    private static AttackManager instance;
    public static AttackManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<AttackManager>();
            }
            return instance;
        }
    }

    [Header("相机")]
    [SerializeField] private CinemachineVirtualCamera camer;
    private CinemachineBasicMultiChannelPerlin perlin;

    [Header("连击数")]
    [SerializeField] private GameObject comboPanel;
    [SerializeField] private Text comboText;
    [SerializeField] private float interval;
    [SerializeField] private int combo = 0;
    public int Combo
    {
        get { return combo; }
        set
        {
            combo = value;
            comboText.text = combo.ToString();
            comboPanel.SetActive(true);
            timer = interval;
        }
    }

    private float timer = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (camer == null)
        {
            Debug.LogError("未设置虚拟相机");
        }
        perlin = camer.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        comboPanel.SetActive(false);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f && Combo != 0)
        {
            Combo = 0;
            comboPanel.SetActive(false);
        }
    }

    public void ComboUp(PlayerAttackBox attackBox)
    {
        Combo++;

        if(attackBox.UseCameraShake)
        {
            ShakeCamera(attackBox.CameraShakeDuration, attackBox.CameraShakeStrenght);
        }

        if(attackBox.UseSudden)
        {
            SuddenCamera(attackBox.SuddenDuration);
        }
    }

    // 镜头晃动效果 ----------------------------------------------
    public void ShakeCamera(float duration, float strength)
    {
        StartCoroutine(ShakeCameraCo(duration, strength));
    }

    IEnumerator ShakeCameraCo(float duration, float strength)
    {
        perlin.m_AmplitudeGain = strength;
        perlin.m_FrequencyGain = 100;

        yield return new WaitForSeconds(duration);

        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 100;
    }

    // 镜头卡顿效果 -----------------------------------------------
    public void SuddenCamera(float duration)
    {
        StartCoroutine(SuddenlyCo(duration));
    }

    IEnumerator SuddenlyCo(float duration)
    {
        Time.timeScale = 0.5f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1;
    }

    // 镜头缩进效果 -----------------------------------------------
    private bool _onChangeFov = false;

    public void ChangeCameraFov(float targetFOV)
    {
        StartCoroutine(ChangeCameraFovCo(targetFOV));
    }
 
    IEnumerator ChangeCameraFovCo(float targetFOV)
    {
        // 获取相机当前的FOV
        float originalFOV = camer.m_Lens.FieldOfView;

        if (_onChangeFov) yield break;
        if (originalFOV == targetFOV) yield break;

        float elapsedTime = 0f;
        _onChangeFov = true;

        while (elapsedTime < 1)
        {
            camer.m_Lens.FieldOfView = Mathf.Lerp(originalFOV, targetFOV, (elapsedTime / 1));
            elapsedTime += Time.unscaledDeltaTime; 
            yield return null;
        }

        // 将相机FOV设为最终值
        camer.m_Lens.FieldOfView = targetFOV;
        _onChangeFov = false;
    }
}

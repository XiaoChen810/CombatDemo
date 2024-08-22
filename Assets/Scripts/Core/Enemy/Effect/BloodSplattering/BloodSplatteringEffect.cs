using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatteringEffect : MonoBehaviour
{
    private static readonly string s_bloodEffectSettingDefaultPath = "BloodSplatteringEffect/BloodEffectSettingDefault";

    [SerializeField] private BloodEffectSetting bloodEffectSetting;

    private List<ParticleSystem> bloodEffect;

    private float lastTriggerTime = -1;
    private float triggerTimeInterval = 0.75f;
    private void Start()
    {
        if (bloodEffectSetting == null)
        {
            bloodEffectSetting = Resources.Load<BloodEffectSetting>(s_bloodEffectSettingDefaultPath);
            if (bloodEffectSetting == null)
            {
                Debug.LogError($"无法加载默认的血液效果设置：{s_bloodEffectSettingDefaultPath}");
                return;
            }
        }

        bloodEffect = bloodEffectSetting.bloodEffect;

        if (bloodEffect == null || bloodEffect.Count == 0)
        {
            Debug.LogError("血液效果列表为空或未初始化！");
        }
    }

    public void Splatter()
    {
        if (bloodEffect == null || bloodEffect.Count == 0)
        {
            Debug.LogError("无法播放血液飞溅特效，因为效果列表为空或未初始化！");
            return;
        }

        if (Time.time < lastTriggerTime + triggerTimeInterval)
        {
            return;
        }

        lastTriggerTime = Time.time;

        int random = Random.Range(0, bloodEffect.Count);
        var effect = bloodEffect[random];
        if (effect != null)
        {
            // 实例化特效对象并播放
            Instantiate(effect, transform.position, Quaternion.identity).Play();
        }
        else
        {
            Debug.LogError("选择的血液效果为 null，无法播放特效！");
        }
    }
}

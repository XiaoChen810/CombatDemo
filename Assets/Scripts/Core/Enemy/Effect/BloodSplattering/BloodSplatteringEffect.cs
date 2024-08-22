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
                Debug.LogError($"�޷�����Ĭ�ϵ�ѪҺЧ�����ã�{s_bloodEffectSettingDefaultPath}");
                return;
            }
        }

        bloodEffect = bloodEffectSetting.bloodEffect;

        if (bloodEffect == null || bloodEffect.Count == 0)
        {
            Debug.LogError("ѪҺЧ���б�Ϊ�ջ�δ��ʼ����");
        }
    }

    public void Splatter()
    {
        if (bloodEffect == null || bloodEffect.Count == 0)
        {
            Debug.LogError("�޷�����ѪҺ�ɽ���Ч����ΪЧ���б�Ϊ�ջ�δ��ʼ����");
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
            // ʵ������Ч���󲢲���
            Instantiate(effect, transform.position, Quaternion.identity).Play();
        }
        else
        {
            Debug.LogError("ѡ���ѪҺЧ��Ϊ null���޷�������Ч��");
        }
    }
}

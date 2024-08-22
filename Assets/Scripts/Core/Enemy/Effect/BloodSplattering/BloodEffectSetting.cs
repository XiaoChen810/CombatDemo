using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodEffectSettingDefault", menuName = "Effect/BloodSplatteringEffect")]
public class BloodEffectSetting : ScriptableObject
{
    public List<ParticleSystem> bloodEffect;
}

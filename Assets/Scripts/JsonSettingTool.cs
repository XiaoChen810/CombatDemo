using ChenChen_Core;
using System.IO;
using UnityEngine;

public class JsonSettingTool : MonoBehaviour
{
    [SerializeField] private RedHood redHood;
    [SerializeField] private Samurai samurai;

    public void Start()
    {
        if (redHood == null)
        {
            Debug.LogError("Error not redHood Component !");
            return;
        }

        ApplyKeyCodeBindingToRedHood(redHood);
        ApplySettingsToRedHood(redHood);
        ApplySettingsToSamurai(samurai);
    }

    private static void ApplyKeyCodeBindingToRedHood(RedHood redHood)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "keybindings.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            KeyBindings keyBindings = JsonUtility.FromJson<KeyBindings>(json);

            redHood.jumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.jumpKey);
            redHood.ligthHitOrBowHitKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.lightHitOrBowHitKey);
            redHood.heavyHitKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.heavyHitKey);
            redHood.slidingKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.slidingKey);
            redHood.dodgeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.dodgeKey);
            redHood.keySkillFirst = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.keySkillFirst);
            redHood.keySkillSecond = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.keySkillSecond);
            redHood.keySkillSpecialSkill = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyBindings.keySkillSpecialSkill);
        }
        else
        {
            Debug.LogError("Key bindings file not found");
        }
    }

    private static void ApplySettingsToRedHood(RedHood redHood)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "playersetting.json");

        PlayerSetting settings = LoadPlayerSetting(filePath);

        if (settings == null || redHood == null)
        {
            Debug.LogError("settings == null || redHood == null");
        }

        redHood.MaxHealth = settings.MaxHealth;
        redHood.MaxPower = settings.MaxPower;
        redHood.MoveSpeed = settings.MoveSpeed;
        redHood.jumpForceSmall = settings.JumpForce.Small;
        redHood.jumpForceBig = settings.JumpForce.Big;
        redHood.ligthHitStrength = settings.HitStrength.Light;
        redHood.heavyHitStrength = settings.HitStrength.Heavy;
        redHood.slidingSpeed = settings.SlidingSpeed;
        redHood.dodgeDuration = settings.Dodge.Duration;
        redHood.dodgeCooldownTime = settings.Dodge.CooldownTime;
        redHood.dodgeSpeed = settings.Dodge.Speed;
        redHood.hurtCooldownTime = settings.HurtCooldownTime;
        redHood.power_skill1 = settings.SkillPowerConsumption.Skill1;
        redHood.power_skill2 = settings.SkillPowerConsumption.Skill2;
        redHood.power_SpecialSkill = settings.SkillPowerConsumption.SpecialSkill;
    }

    public static void ApplySettingsToSamurai(Samurai samurai)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "enemysetting.json");

        SamuraiSetting settings = LoadSamuraiSetting(filePath);

        if (settings == null || samurai == null)
        {
            Debug.LogError("settings == null || samurai == null");
        }

        samurai.MaxHealth = settings.MaxHealth;
        samurai.defendProbability = settings.DefendProbability;
        samurai.firstStagePercentage = settings.StagePercentage.FirstStage;
        samurai.secondStagePercentage = settings.StagePercentage.SecondStage;
        samurai.thirdStagePercentage = settings.StagePercentage.ThirdStage;
    }

    private static PlayerSetting LoadPlayerSetting(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerSetting>(json);
        }
        else
        {
            Debug.LogError("Player setting file not found: " + filePath);
            return null;
        }
    }

    public static SamuraiSetting LoadSamuraiSetting(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<SamuraiSetting>(json);
        }
        else
        {
            Debug.LogError("Samurai setting file not found: " + filePath);
            return null;
        }
    }
}

#region - Player Setting -
[System.Serializable]
public class PlayerSetting
{
    public float MaxHealth = 100;
    public float MaxPower = 100;
    public float MoveSpeed;
    public JumpForce JumpForce;
    public HitStrength HitStrength;
    public float SlidingSpeed = 15f;
    public Dodge Dodge;
    public float HurtCooldownTime = 0.5f;
    public SkillPowerConsumption SkillPowerConsumption;
}

[System.Serializable]
public class JumpForce
{
    public float Small;
    public float Big;
}

[System.Serializable]
public class HitStrength
{
    public float Light = 1.1f;
    public float Heavy = 1.5f;
}

[System.Serializable]
public class Dodge
{
    public float Duration = 0.5f;
    public float CooldownTime = 0.6f;
    public float Speed = 15f;
}

[System.Serializable]
public class SkillPowerConsumption
{
    public float Skill1 = 25;
    public float Skill2 = 50;
    public float SpecialSkill = 75;
}
#endregion

#region - Key Bindings -

[System.Serializable]
public class KeyBindings
{
    public string jumpKey;
    public string lightHitOrBowHitKey;
    public string heavyHitKey;
    public string slidingKey;
    public string dodgeKey;
    public string keySkillFirst;
    public string keySkillSecond;
    public string keySkillSpecialSkill;
}

#endregion

#region - Enemy Setting -
[System.Serializable]
public class SamuraiSetting
{
    public float MaxHealth = 100;
    public float DefendProbability = 0.7f;
    public StagePercentage StagePercentage;
}

[System.Serializable]
public class StagePercentage
{
    [Range(0f, 1f)] public float FirstStage = 0.9f;
    [Range(0f, 1f)] public float SecondStage = 0.7f;
    [Range(0f, 1f)] public float ThirdStage = 0.5f;
}
#endregion


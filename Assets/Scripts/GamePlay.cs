using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    [Header("按键绑定")]
    public GameObject BindingPanel;
    public GameObject InputMessagePanel;
    public Text t_jumpKey;
    public Text t_lightHitOrBowHitKey;
    public Text t_heavyHitKey;
    public Text t_slidingKey;
    public Text t_dodgeKey;
    public Text t_keySkillFirst;
    public Text t_keySkillSecond;
    public Text t_keySkillSpecialSkill;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame_Night()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartGame_Day()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    #region - 更新键位 -

    private KeyBindings keyBindings = null;

    private string waitingForKeyChange = string.Empty;

    private void Update()
    {
        // Binding KeyCode
        if (!string.IsNullOrEmpty(waitingForKeyChange) && Input.anyKey)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    ChangeKeyBinding(waitingForKeyChange, keyCode.ToString());
                    waitingForKeyChange = string.Empty;  // 重置等待的键绑定
                    InputMessagePanel.SetActive(false);
                    UpdateBindingKeyCodeText();
                    break;
                }
            }
        }
    }

    private void UpdateBindingKeyCodeText()
    {
        if (keyBindings != null)
        {
            t_jumpKey.text = keyBindings.jumpKey;
            t_lightHitOrBowHitKey.text = keyBindings.lightHitOrBowHitKey;
            t_heavyHitKey.text = keyBindings.heavyHitKey;
            t_slidingKey.text = keyBindings.slidingKey;
            t_dodgeKey.text = keyBindings.dodgeKey;
            t_keySkillFirst.text = keyBindings.keySkillFirst;
            t_keySkillSecond.text = keyBindings.keySkillSecond;
            t_keySkillSpecialSkill.text = keyBindings.keySkillSpecialSkill;
        }
    }

    public void OpenBindingPanel()
    {
        GetKeyBindingsFromJson();
        BindingPanel.SetActive(true);
        UpdateBindingKeyCodeText();
    }

    public void CloseBindingPanel()
    {
        BindingPanel.SetActive(false);
    }

    public void SetWaitingForKeyChange(string changed)
    {
        waitingForKeyChange = changed;

        InputMessagePanel.SetActive(true);
    }

    public void ApplyNewSetting()
    {
        string updatedJson = JsonUtility.ToJson(keyBindings);
        string filePath = Path.Combine(Application.streamingAssetsPath, "keybindings.json");
        File.WriteAllText(filePath, updatedJson);

        CloseBindingPanel();
    }

    private void ChangeKeyBinding(string changed, string newKeyCode)
    {
        if (keyBindings != null)
        {
            var field = keyBindings.GetType().GetField(changed);
            if (field != null)
            {
                field.SetValue(keyBindings, newKeyCode); 
            }
        }
    }

    private void GetKeyBindingsFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "keybindings.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            keyBindings = JsonUtility.FromJson<KeyBindings>(json);
        }
        else
        {
            Debug.LogError("Key bindings file not found");
        }
    }

    #endregion

}
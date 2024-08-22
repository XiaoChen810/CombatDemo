using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_Core;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private MeshCollider mapBorder;

    [SerializeField] private Slider healthBarPlayer;
    [SerializeField] private Slider healthBarBoss;
    [SerializeField] private Slider powerBarPlayer;

    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelDefeat;
    [SerializeField] private GameObject panelVictory;

    [SerializeField] private Samurai samurai;
    [SerializeField] private RedHood redHood;

    [SerializeField] private float totalGameTime;

    private bool gameIsEnd = false;

    public enum GameDifficultyType
    {
        Normal, Difficult
    }
    public GameDifficultyType GameDifficulty = GameDifficultyType.Normal;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        panelMenu.SetActive(false);
        panelDefeat.SetActive(false);
        panelVictory.SetActive(false);
    }

    private void Update()
    {
        if (gameIsEnd) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panelMenu.SetActive(!panelMenu.activeSelf);
        }

        healthBarPlayer.value = redHood.Hp / redHood.MaxHealth;
        healthBarBoss.value = samurai.Hp / samurai.MaxHealth;
        powerBarPlayer.value = redHood.Power / redHood.MaxPower;

        totalGameTime += Time.deltaTime;

        if (redHood.Hp <= 0)
        {
            Defeat();
            gameIsEnd = true;
            return;
        }

        if (samurai.Hp <= 0)
        {
            Victory();
            gameIsEnd = true;
            return;
        }
    }

    public bool CheckPosition(Vector3 position)
    {
        return mapBorder.bounds.Contains(position);
    }

    public void Defeat()
    {
        panelDefeat.SetActive(true);
    }

    public void Victory()
    {
        panelVictory.SetActive(true);

        // 计算总游戏时间的小时、分钟和秒
        int hours = Mathf.FloorToInt(totalGameTime / 3600);
        int minutes = Mathf.FloorToInt((totalGameTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalGameTime % 60);

        string content;

        // 判断小时数是否为0，如果为0，则只显示分钟和秒
        if (hours > 0)
        {
            content = string.Format("用时\n{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }
        else
        {
            content = string.Format("用时\n{0:D2}:{1:D2}", minutes, seconds);
        }

        // 更新UI中的时间显示
        panelVictory.transform.Find("时间").GetComponentInChildren<Text>().text = content;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReStart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpDifficulty()
    {
        GameDifficulty = GameDifficultyType.Difficult;
    }
}

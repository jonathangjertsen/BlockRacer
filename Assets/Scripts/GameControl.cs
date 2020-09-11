﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    private static bool created = false;
    private static bool restartIncoming = false;

    public static int level = 0;

    public float restartDelay;
    public GameObject winScreen;
    public GameObject failScreen;

    private static bool gameOver;
    private static bool win;

    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            Init();
        }
        created = true;
    }

    private void Init()
    {
        gameOver = false;
        win = false;
    }

    public void Win()
    {
        if (win || gameOver)
        {
            return;
        }
        win = true;

        WinScreen ws = FindObjectOfType<WinScreen>(true);
        if (ws)
        {
            ws.gameObject.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (win || gameOver)
        {
            return;
        }

        gameOver = true;
        FailScreen fs = FindObjectOfType<FailScreen>(true);
        if (fs)
        {
            fs.gameObject.SetActive(true);
        }
        Restart();
    }

    public void Restart()
    {
        if (restartIncoming)
        {
            return;
        }
        restartIncoming = true;
        Invoke("RestartNow", restartDelay);
    }

    public void RestartNow()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        restartIncoming = false;
        Init();
    }

    public void NextLevel()
    {
        level += 1;
        Restart();
    }

    public void LoadMenu()
    {
        Debug.Log("Loaded menu");
    }
}

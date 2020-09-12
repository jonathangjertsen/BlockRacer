﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    private static bool created = false;
    private static bool restartIncoming = false;

    public AudioClip goldClip;
    public AudioClip silverClip;
    public AudioClip bronzeClip;
    public AudioClip winClip;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource dieSource;
    public AudioSource jumpSource;

    public static int level = 0;

    public float restartDelay;

    private static bool gameOver;
    private static bool win;
    private static bool paused;

    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            Init();
            musicSource.Play();
        }
        created = true;
    }

    private void Init()
    {
        gameOver = false;
        win = false;

        Player player = Player.Find();
        if (player)
        {
            player.OnWin += Win;
            player.OnDie += GameOver;
            player.OnJump += Jump;
        }
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        Player player = Player.Find();
        if (player)
        {
            int score = player.GetScore();
            ScoreTarget.CheckTargets(score);
        }
    }

    public void Jump()
    {
        jumpSource.Play();
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

        if (ScoreTarget.GetAchieved("gold"))
        {
            sfxSource.PlayOneShot(goldClip);
        }
        else if (ScoreTarget.GetAchieved("silver"))
        {
            sfxSource.PlayOneShot(silverClip);
        }
        else if (ScoreTarget.GetAchieved("bronze"))
        {
            sfxSource.PlayOneShot(bronzeClip);
        }
        else
        {
            sfxSource.PlayOneShot(winClip);
        }
    }

    public void GameOver()
    {
        if (win || gameOver)
        {
            return;
        }

        gameOver = true;

        dieSource.Play();

        FailScreen fs = FindObjectOfType<FailScreen>(true);
        if (fs)
        {
            fs.gameObject.SetActive(true);
        }
        Restart();
    }

    public void Pause()
    {
        if (paused)
        {
            return;
        }

        paused = true;
        Time.timeScale = 0;
        PauseScreen ps = FindObjectOfType<PauseScreen>(true);
        if (ps)
        {
            ps.gameObject.SetActive(true);
        }
    }

    public void Unpause()
    {
        if (!paused)
        {
            return;
        }

        paused = false;
        Time.timeScale = 1;
        PauseScreen ps = FindObjectOfType<PauseScreen>(true);
        if (ps)
        {
            ps.gameObject.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (paused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
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

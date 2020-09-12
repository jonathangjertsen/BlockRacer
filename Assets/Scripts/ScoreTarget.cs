using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreTarget : MonoBehaviour
{
    public string match;
    public TMP_Text text;
    public Image medal;
    public Color notAchievedColor;
    public Color achievedColor;
    private int score;
    private bool achieved;

    void Start()
    {
        medal.color = notAchievedColor;
        text.color = notAchievedColor;
        achieved = false;
    }

    public static void SetTarget(string match, int score)
    {
        foreach (ScoreTarget target in FindObjectsOfType<ScoreTarget>(true))
        {
            if (match == target.match && target.text)
            {
                target.score = score;
                target.text.text = score.ToString();
            }
        }
    }

    public static void CheckTargets(int score)
    {
        foreach (ScoreTarget target in FindObjectsOfType<ScoreTarget>(true))
        {
            if (!target.achieved && score > target.score)
            {
                target.SetAchieved();
            }
        }
    }

    void SetAchieved()
    {
        medal.color = achievedColor;
        text.color = achievedColor;
        achieved = true;
    }

    public static bool GetAchieved(string match)
    {
        foreach (ScoreTarget target in FindObjectsOfType<ScoreTarget>(true))
        {
            if (match == target.match)
            {
                return target.achieved;
            }
        }
        return false;
    }
}

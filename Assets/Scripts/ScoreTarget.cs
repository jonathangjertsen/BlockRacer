using UnityEngine;
using TMPro;

public class ScoreTarget : MonoBehaviour
{
    public string match;
    public TMP_Text text;

    public static void SetTarget(string match, int score)
    {
        foreach (ScoreTarget target in FindObjectsOfType<ScoreTarget>(true))
        {
            if (match == target.match && target.text)
            {
                target.text.text = score.ToString();
            }
        }
    }
}

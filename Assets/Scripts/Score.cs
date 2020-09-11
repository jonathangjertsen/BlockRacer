using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text text;

    void Update()
    {
        int score = FindObjectOfType<Player>().UpdateScore();
        text.text = score.ToString();
    }
}

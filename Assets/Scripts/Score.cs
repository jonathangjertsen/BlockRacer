using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text text;
    public int score;

    void FixedUpdate()
    {
        score = FindObjectOfType<Player>().UpdateScore();
    }

    private void Update()
    {
        text.text = score.ToString();
    }
}

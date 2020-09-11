using UnityEngine;
using TMPro;

public class Speed : MonoBehaviour
{
    public TMP_Text text;

    void Update()
    {
        text.text = FindObjectOfType<Player>().GetSpeed().ToString();
    }
}

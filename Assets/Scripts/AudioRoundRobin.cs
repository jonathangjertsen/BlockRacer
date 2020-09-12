using UnityEngine;

[System.Serializable]
public class AudioRoundRobin
{
    public AudioClip[] clips;
    public AudioSource source;
    public int index = 0;

    public void Play()
    {
        if (clips.Length == 0)
        {
            return;
        }
        if (index >= clips.Length)
        {
            index = 0;
        }
        source.PlayOneShot(clips[index]);
        index++;
    }
}

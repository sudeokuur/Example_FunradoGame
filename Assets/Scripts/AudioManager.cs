using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource soundEffectSource;
    public AudioClip yourSoundEffectClip;

    void Start()
    {
        PlaySoundEffect();
    }

    public void PlaySoundEffect()
    {
        if (soundEffectSource != null && yourSoundEffectClip != null)
        {
            soundEffectSource.clip = yourSoundEffectClip;

            soundEffectSource.Play();
        }
        else
        {
            Debug.LogWarning("Sound effect source or clip is not assigned!");
        }
    }
}

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip buttonPressClip;
    public AudioClip menuSwapClip;

    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup musicMixerGroup;

    private AudioSource sfxAudioSource;
    private AudioSource musicAudioSource;

    void Start()
    {
        // Create audio sources for SFX and music
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource = gameObject.AddComponent<AudioSource>();

        // Assign the Audio Mixer Groups to the respective audio sources
        sfxAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        musicAudioSource.outputAudioMixerGroup = musicMixerGroup;
    }

    public void PlayButtonPress()
    {
        PlaySound(buttonPressClip, sfxAudioSource);
    }

    public void PlayMenuSwap()
    {
        PlaySound(menuSwapClip, sfxAudioSource);
    }

    private void PlaySound(AudioClip clip, AudioSource audioSource)
    {
        audioSource.PlayOneShot(clip);
    }
}

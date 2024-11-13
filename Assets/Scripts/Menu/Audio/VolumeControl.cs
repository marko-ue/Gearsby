using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer masterMixer;


    public void SetMasterLevel(float _volume)
    {
        masterMixer.SetFloat("MasterParam", _volume);
    }

    public void SetSoundLevel(float _volume)
    {
        masterMixer.SetFloat("SoundParam", _volume);
    }

    public void SetMusicLevel(float _volume)
    {
        masterMixer.SetFloat("MusicParam", _volume);
    }
}
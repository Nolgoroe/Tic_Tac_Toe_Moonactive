using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    ButtonClick,
    MarkCell,
    Win,
    Timeout,
    MenuMusic,
    GameMusic,
}

[System.Serializable]
public class AudioSourceCombo
{
    public AudioSource source;
    public float maxVolume;
    public float timeToFadeVolume;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] List<AudioSourceCombo> allAudioSources;

    [Range(0f, 1f)]
    [SerializeField] private float soundVolumeModifier;

    private Dictionary<Sounds, AudioSourceCombo> SoundToAudioSourceDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SoundToAudioSourceDict = new Dictionary<Sounds, AudioSourceCombo>();

        for (int i = 0; i < allAudioSources.Count; i++)
        {
            SoundToAudioSourceDict.Add((Sounds)i, allAudioSources[i]);
        }
    }

    #region Play Sounds
    public void PlaySoundFade(Sounds sound)
    {
        float time = SoundToAudioSourceDict[sound].source.clip.length;

        SoundToAudioSourceDict[sound].source.volume = 0;

        SoundToAudioSourceDict[sound].source.Play();

        LeanTween.value(SoundToAudioSourceDict[sound].source.gameObject, 0, SoundToAudioSourceDict[sound].maxVolume * soundVolumeModifier, SoundToAudioSourceDict[sound].timeToFadeVolume).setOnUpdate((float val) =>
        {
            SoundToAudioSourceDict[sound].source.volume = val;
        });
    }

    public void PlaySoundOneShot(Sounds sound)
    {
        SoundToAudioSourceDict[sound].source.PlayOneShot(SoundToAudioSourceDict[sound].source.clip, SoundToAudioSourceDict[sound].maxVolume * soundVolumeModifier);
    }
    public void PlaySoundNormal(Sounds sound)
    {
        SoundToAudioSourceDict[sound].source.volume = soundVolumeModifier;
        SoundToAudioSourceDict[sound].source.Play();
    }
    #endregion

    #region Stop Sounds
    public void StopSoundFade(Sounds sound)
    {
        LeanTween.value(SoundToAudioSourceDict[sound].source.gameObject, SoundToAudioSourceDict[sound].source.volume * soundVolumeModifier, 0, SoundToAudioSourceDict[sound].timeToFadeVolume).setOnUpdate((float val) =>
        {
            SoundToAudioSourceDict[sound].source.volume = val;
        });
    }
    public void StopSound(Sounds sound)
    {
        if (SoundToAudioSourceDict[sound].source.isPlaying)
            SoundToAudioSourceDict[sound].source.Stop();
    }
    #endregion

    #region Volume Control
    public void OnChangeSoundVolumeModifier(float value)
    {
        soundVolumeModifier = value;

        foreach (AudioSourceCombo combo in allAudioSources)
        {
            combo.source.volume = combo.maxVolume * soundVolumeModifier;
        }
    }
    #endregion
}

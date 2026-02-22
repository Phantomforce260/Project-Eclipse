using System;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /* The AudioManager is responsible for storing and playing sounds.
     * It stores music and SFX in separate arrays, each accessible through their own methods.
     * It references and controls the AudioMixers, which control volume. */
    public static Sound CurrentPlayingSound;

    public const float MaxVolume = 0;
    public const float MinVolume = -80;

    private static AudioManager instance;

    [SerializeField] private Sound[] soundEffects;
    [SerializeField] private Sound[] music;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup sfxMaster;
    [SerializeField] private AudioMixerGroup musicMaster;

    public enum SoundType
    { Music, SFX }

    /* Awake:
     * 
     * Initialize the singleton. If one already exists, destroy this object. */
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        UpdateSounds();
    }

    /* UpdateSounds:
     * 
     * Adds an AudioSource component to this game object for every Sound object in both arrays.
     * Sets the variables of each AudioSource to its respective Sound object. */
    private void UpdateSounds()
    {
        foreach (Sound sound in soundEffects)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sfxMaster;
        }

        foreach (Sound sound in music)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = musicMaster;
        }
    }

    /* PlaySFX --> PlaySFXInstance:
     *     - soundName: The name of the sfx clip. Assigned in the inspector.
     * 
     * Finds a sound effect by its name and plays it, if it exists. */
    public static void PlaySFX(string soundName) => instance.PlaySFXInstance(soundName);

    private void PlaySFXInstance(string soundName)
    {
        Sound soundEffect = GetSoundInstance(soundName, SoundType.SFX);
        if (soundEffect != null)
        {
            soundEffect.source.volume = soundEffect.volume;
            soundEffect.source.Play();
        }
    }

    /* PlayMusic --> PlayMusicInstance:
     *     - trackName: The name of the music track. Assigned in the inspector.
     *     
     * Finds a music track by its name and plays it, if it exists. */
    public static void PlayMusic(string trackName, bool hasIntro = false) => instance.PlayMusicInstance(trackName, hasIntro);

    private void PlayMusicInstance(string trackName, bool hasIntro)
    {
        if (hasIntro)
        {
            Sound intro = GetSoundInstance($"{trackName} (Intro)", SoundType.Music);
            Sound loop = GetSoundInstance($"{trackName} (Loop)", SoundType.Music);

            if (intro != null && loop != null)
            {
                intro.source.volume = intro.volume;
                loop.source.volume = loop.volume;

                intro.source.Play();

                loop.source.PlayScheduled(AudioSettings.dspTime + intro.source.clip.length);
            }
        }
        else
        {
            Sound music = GetSoundInstance(trackName, SoundType.Music);
            if (music != null)
            {
                music.source.volume = music.volume;
                music.source.Play();
            }
        }
    }

    public static void SwitchTracks(string target) => CurrentPlayingSound = instance.SwitchTracksInstance(target);

    private Sound SwitchTracksInstance(string target)
    {
        Sound targetTrack = GetSoundInstance(target, SoundType.Music);
        if (targetTrack != null)
            StartCoroutine(SwitchTracksCoroutine(CurrentPlayingSound, targetTrack));

        return targetTrack;
    }

    private IEnumerator SwitchTracksCoroutine(Sound current, Sound target)
    {
        target.source.volume = 0;
        while (target.source.volume < 0.98f)
        {
            current.source.volume -= Time.deltaTime;
            target.source.volume += Time.deltaTime;
            yield return null;
        }

        current.source.volume = 0;
        target.source.volume = 0.5f;
    }

    /* StopSFX --> StopSFXInstance:
     *
     * Stops all sound effects. */
    public static void StopSFX() => instance.StopSFXInstance();

    private void StopSFXInstance()
    {
        foreach (Sound sound in soundEffects.Where(s => s.source.isPlaying))
            sound.source.Stop();
    }

    /* StopMusic --> StopMusicInstance:
     *     - durationSeconds: The amount of time for the track to fade out. Default value of 1 second.
     *     
     * Fades out all music. Fade out duration can be specified. */
    public static void StopMusic(float durationSeconds = 1) => instance.StopMusicInstance(durationSeconds);

    private void StopMusicInstance(float durationSeconds)
    {
        foreach (Sound sound in music.Where(s => s.source.isPlaying))
        {
            StartCoroutine(Diminuendo(sound, durationSeconds));
            Invoke(nameof(StopMusicCoroutine), durationSeconds);
        }
    }

    /* Diminuendo:
     *     - track: The sound to diminuendo.
     *     - durationSeconds: The amount of time for the track to fade out. Default value of 1 second.
     * 
     * Used for fading out music.
     * Linearly interpolates the volume of the track in a decrementing fashion, until the volume is 0.
     * When finished, stop the track. */
    private IEnumerator Diminuendo(Sound track, float durationSeconds)
    {
        float elapsedTime = 0;
        float currentVolume = track.source.volume;

        while (track.source.volume >= 0 && elapsedTime < durationSeconds)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(currentVolume, 0, elapsedTime / durationSeconds);

            track.source.volume = newVolume;
            yield return null;
        }

        track.source.Stop();
    }

    private void StopMusicCoroutine() => StopCoroutine(nameof(Diminuendo));

    /* PauseMusic --> PauseMusicInstance:
     *     - trackName: The name of the music track. Assigned in the inspector.
     *     
     * Finds a music track by its name and pauses it, if it exists and is playing. */
    public static void PauseMusic(string trackName) => instance.PauseMusicInstance(trackName);

    private void PauseMusicInstance(string trackName)
    {
        Sound music = GetSoundInstance(trackName, SoundType.Music);
        if (music != null && music.source.isPlaying)
            music.source.Pause();
    }

    /* PauseSFX --> PauseSFXInstance:
     *     - soundName: The name of the SFX clip. Assigned in the inspector.
     *  
     * Finds a sound effect by its name and pauses it, if it exists and is playing. */
    public static void PauseSFX(string soundName) => instance.PauseSFXInstance(soundName);

    private void PauseSFXInstance(string soundName)
    {
        Sound soundEffect = GetSoundInstance(soundName, SoundType.SFX);
        if (soundEffect != null && soundEffect.source.isPlaying)
            soundEffect.source.Pause();
    }

    /* IsSoundPlaying --> IsSoundPlayingInstance:
     *     - soundName: The name of the sound. Assigned in the inspector.
     *     - soundType: The type of sound (Music or SFX). Determines which list to check.
     *     
     * Finds a sound by its name and type and checks if it is playing.
     * This method is mostly called by other classes. */
    public static bool IsSoundPlaying(string soundName, SoundType soundType) => instance.IsSoundPlayingInstance(soundName, soundType);

    private bool IsSoundPlayingInstance(string soundName, SoundType soundType)
    {
        Sound[] soundArray = soundType == SoundType.Music ? music : soundEffects;
        return soundArray.Any(s => s.name.Equals(soundName) && s.source.isPlaying);
    }

    /* SetVolume --> SetVolumeInstance:
     *     - volume: The new volume value. Must be within [-80, -1], inclusive.
     *     
     * Sets the master volume. 
     * NOTE: Individual mixer volume is set in SettingsMenu.cs */
    public static void SetVolume(float volume) => instance.SetVolumeInstance(volume);

    private void SetVolumeInstance(float volume)
    {
        if (volume < MinVolume || volume > MaxVolume)
            volume = MinVolume;

        audioMixer.SetFloat("MasterVolume", volume);
    }

    /* Mute: 
     * 
     * Sets the master volume to 0. */
    public static void Mute() => instance.SetVolumeInstance(0);

    /* GetSound --> GetSoundInstance:
     *     - soundName: The name of the sound. Assigned in the inspector.
     *     - soundType: The type of sound (Music or SFX). Determines which list to check.
     *     
     * Gets a sound from the Sound Object arrays, by its name and type.
     * If it does not exist, returns null. */
    public static Sound GetSound(string soundName, SoundType soundType) => instance.GetSoundInstance(soundName, soundType);

    private Sound GetSoundInstance(string soundName, SoundType soundType)
    {
        Sound sound = Array.Find(soundType == SoundType.Music ? music : soundEffects, s => s.name.Equals(soundName));
        if (sound == null)
        {
            Debug.LogWarning($"Sound: {soundName} does not exist!");
            return null;
        }
        return sound;
    }
}
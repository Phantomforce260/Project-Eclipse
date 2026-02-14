using System;
using UnityEngine.Audio;
using UnityEngine;

[Serializable]
public class Sound
{
    /* A basic script that stores information about a sound, including:
     * 
     * - Its name (by which it is accessed via the AudioManager)
     * - Its AudioClip (the actual sound in the Project Files)
     * 
     * - Its volume, which is referenced when creating the sound.
     * - Its pitch, which is referenced when creating the sound.
     * - Whether the sound loops.
     * 
     * - The AudioSource component that gets attached to the AudioManager gameObject.
     *   This is created by the AudioManager at runtime. 
     */

    public string name;

    public AudioClip clip;

    [Range(0, 1)]
    public float volume;

    [Range(0.1f, 3)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRandom : MonoBehaviour
{
    public Vector2 volumeRange;
    public Vector2 pitchRange;
    AudioSource _mAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        _mAudioSource = GetComponent<AudioSource>();
        _mAudioSource.pitch = (Random.Range(pitchRange.x, pitchRange.y));
        _mAudioSource.volume = (Random.Range(volumeRange.x, volumeRange.y));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

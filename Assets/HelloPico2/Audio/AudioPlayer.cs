using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Audio
{
    public static class AudioPlayer
    {
        public static void PlayAudio(string ClipName, AudioSource source) {
            source.PlayOneShot(GameManager.GameManager.Instance._SFXLibrary.GetPlayerControllerClip(ClipName));
        }
    }
}

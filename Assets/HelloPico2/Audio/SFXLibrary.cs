using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Audio
{
    [CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/SFX Library")]
    public class SFXLibrary : ScriptableObject
    {        
        [System.Serializable]
        public struct TrackData {
            public string ID;
            public string Name;
            public string Description;
            public AudioClip Clip;
        }
        public List<TrackData> _PlayerControllerTracks = new List<TrackData>();

        private AudioClip GetClip(List<TrackData> trackList, string Name) {

            for (int i = 0; i < trackList.Count; i++)
            {
                if (trackList[i].Name == Name)
                    return trackList[i].Clip;
            }

            Debug.Log("Couldn't find track: " + Name);
            return null;
        }

        public AudioClip GetPlayerControllerClip(string Name) {
            return GetClip(_PlayerControllerTracks, Name);
        }
    }
}

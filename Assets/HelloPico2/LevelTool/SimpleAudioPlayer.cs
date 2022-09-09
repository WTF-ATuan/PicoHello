using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class SimpleAudioPlayer : MonoBehaviour
    {
        [SerializeField] private bool _PlayOnAwake = true;        
        [SerializeField] private string _ClipName;        
        [SerializeField] private bool _UseMultiClips;
        private void Start()
        {
            if (_PlayOnAwake)
                PlayAudio();
        }
        public void PlayAudio() {
            if (!_UseMultiClips)
                AudioPlayerHelper.PlayAudio(_ClipName, transform.position);
            else
            {
                AudioPlayerHelper.PlayMultipleAudio(_ClipName, transform.position);
            }
        }
    }
}
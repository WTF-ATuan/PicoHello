using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class MeshTextureSequence : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _MeshRenderer;
        [SerializeField] private Texture[] _TextureSequence;
        [SerializeField] private int _FPS;
        [SerializeField] private bool _Loop;
        [SerializeField] private bool _CanInterupt;
        public UltEvent WhenFinished;
        Coroutine process;
        public void PlayTextureSequence() {
            if (process != null) {
                if (_CanInterupt)
                    StopCoroutine(process);
                else
                    return;
            }

            process = StartCoroutine(TextureSequencer());
        }
        private IEnumerator TextureSequencer() { 
            int index = 0;
            while (index < _TextureSequence.Length || _Loop) {            
                _MeshRenderer.material.mainTexture = _TextureSequence[index];
                index++;
                if(index >= _TextureSequence.Length && _Loop) {                     
                    index = 0; 
                }
                yield return new WaitForSeconds(1f/(float)_FPS);
            }
            WhenFinished?.InvokeSafe();
        }
    }
}

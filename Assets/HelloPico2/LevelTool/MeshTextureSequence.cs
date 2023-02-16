using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class MeshTextureSequence : MonoBehaviour
    {
        public enum Language { EN, CN}
        [SerializeField] private MeshRenderer _MeshRenderer;
        [SerializeField] public Language _UseLanguage = Language.CN;
        [System.Serializable]
        private struct LogoLanguage { 
            public Language language;
            public Texture[] textureSequence;            
        };
        [SerializeField] private List<LogoLanguage> _LogoImageSequence = new List<LogoLanguage>();
        [SerializeField] private int _FPS;
        [SerializeField] private bool _Loop;
        [SerializeField] private bool _CanInterupt;
        public UltEvent WhenFinished;
        Coroutine process;
        public void ChangeLanguage(Language language) => _UseLanguage = language;
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
            Texture[] textures = _LogoImageSequence.Find(x => x.language == _UseLanguage).textureSequence;

            while (index < textures.Length || _Loop) {            
                _MeshRenderer.material.mainTexture = textures[index];
                index++;
                if(index >= textures.Length && _Loop) {                     
                    index = 0; 
                }
                yield return new WaitForSeconds(1f/(float)_FPS);
            }
            WhenFinished?.InvokeSafe();
        }
    }
}

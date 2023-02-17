using AV.Inspector.Runtime;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

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

        //[ReadOnly][SerializeField] private float _totalPlayTime;
        [ReadOnly][SerializeField] private Texture _currentTexture;
        Coroutine process;
        public void OnValidate()
        {
            //var textures = _LogoImageSequence.Find(x => x.language == _UseLanguage).textureSequence;
            //_totalPlayTime = (1f / (float)_FPS) * textures.Length;
        }
        private void Start(){
            ChangeLanguage(Application.systemLanguage == SystemLanguage.ChineseSimplified ? Language.CN : Language.EN);
        }

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
            //Texture[] textures = _LogoImageSequence.Find(x => x.language == _UseLanguage).textureSequence;
            List<Texture> textures = new List<Texture>(_LogoImageSequence.Find(x => x.language == _UseLanguage).textureSequence);
            var start = Time.time;
            while (index < textures.Count || _Loop) {            
                _MeshRenderer.material.mainTexture = textures[index];
                _currentTexture = textures[index];
                index++;
                if(index >= textures.Count && _Loop) {                     
                    index = 0; 
                }
                yield return new WaitForSeconds(1f/(float)_FPS);
                //yield return new WaitForEndOfFrame();
            }

            WhenFinished?.InvokeSafe();
            //print("Stop Playing " + (Time.time - start));
        }
    }
}

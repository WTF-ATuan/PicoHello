using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HelloPico2.LevelTool
{
    public class VFXFader : MonoBehaviour
    {
        public ParticleSystem[] _VFXs;
        public bool _OverwriteColorOverLifetime = false;
        [ShowIf("_OverwriteColorOverLifetime")] public Gradient _ColorOverLifetimeGradient;
        private List<bool> originalcolorOverLifetime = new List<bool>();
        private List<Gradient> originalGradient = new List<Gradient>();

        private void Start()
        {
            foreach (var vfx in _VFXs) {
                originalcolorOverLifetime.Add(vfx.colorOverLifetime.enabled);
                originalGradient.Add(vfx.colorOverLifetime.color.gradient);
            }
        }
        public void FadeOutVFX() {
            foreach (var vfx in _VFXs)
            {
                var colorOverLifetime = vfx.colorOverLifetime;
                colorOverLifetime.enabled = true;
                
                if(_OverwriteColorOverLifetime)
                    colorOverLifetime.color = _ColorOverLifetimeGradient;

                vfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
        public void ShowVFX() {
            for (int i = 0; i < _VFXs.Length; i++)
            {               
                var colorOverLifetime = _VFXs[i].colorOverLifetime;
                
                colorOverLifetime.enabled = originalcolorOverLifetime[i];

                if (_OverwriteColorOverLifetime)
                {
                    colorOverLifetime.color = originalGradient[i];
                }

                _VFXs[i].Play(true);
            }
        }
    }
}

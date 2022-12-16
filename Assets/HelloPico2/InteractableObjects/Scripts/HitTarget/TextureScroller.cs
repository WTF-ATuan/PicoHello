using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.LevelTool
{
    public class TextureScroller : MonoBehaviour
    {
        public string _TextureName;
        public Renderer[] _Renderers;

        public void StartScrolling(Vector2 from, Vector2 to, float duration) {
            foreach (var renderer in _Renderers) {
                foreach (var mat in renderer.materials)
                {
                    Vector2 offset = from;
                    DOTween.To(() => offset, x => offset = x, to, duration).OnUpdate(() => { 
                        mat.SetTextureOffset(_TextureName ,offset);                    
                    });
                }
            }
        }
    }
}

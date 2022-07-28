using UnityEngine;
using DG.Tweening;

namespace HelloPico2.UI
{
    public class MeshFillAmountController : MonoBehaviour, IImageController
    {
        public MeshRenderer _ControlTarget;
        public string _TilingPropertyName = "_MainTex_ST";
        public string _ColorPropertyName = "_Color";
        public Vector2 _TilingXRange;
        public Vector2 _TilingYRange;
        public float _TilingSpeed = 10;
        public AnimationCurve _EasingCurve;
        public Color _DefaultColor = Color.cyan;
        public Color _MaxColor = Color.red;

        public void UpdateAmount(float amount)
        {
            amount = Mathf.Clamp01(amount);
            //_ControlTarget.material.SetFloat(_FloatPropertyName, (_Range.y - _Range.x) * amount);
            var tileX = Mathf.Lerp(_TilingXRange.x, _TilingXRange.y, _EasingCurve.Evaluate(amount));
            var tileY = Mathf.Lerp(_TilingYRange.x, _TilingYRange.y, _EasingCurve.Evaluate(amount));

            _ControlTarget.material.DOTiling(new Vector4(tileX, tileY, 0, 0), _TilingPropertyName, _TilingSpeed).SetEase(_EasingCurve);

            if (amount < 1)
                _ControlTarget.material.SetColor(_ColorPropertyName, _DefaultColor);
            else
                _ControlTarget.material.SetColor(_ColorPropertyName, _MaxColor);
        }
    }
}

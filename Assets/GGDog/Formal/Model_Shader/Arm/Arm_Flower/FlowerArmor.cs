using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[ExecuteInEditMode]
public class FlowerArmor : MonoBehaviour
{
    public HelloPico2.PlayerController.Arm.ArmData _ArmData;
    [Range(0,1)]
    public float _h;
    [Range(0, 1)]
    public float _Speed = 1;
    public AnimationCurve _EaseCurve;
    public List<Renderer> _m1 = new List<Renderer>();
    public List<Renderer> _m2 = new List<Renderer>();
    public List<Renderer> _m3 = new List<Renderer>();
    public List<Renderer> _m4 = new List<Renderer>();

    public ParticleSystem PS;
    private bool hasTween;
    private void Start()
    {
        Project.EventBus.Subscribe<HelloPico2.InteractableObjects.NeedEnergyEventData>(EnergyUpdate);
    }
    private void EnergyUpdate(HelloPico2.InteractableObjects.NeedEnergyEventData data) {
        if (data.HandType != _ArmData.HandType) return;
        var ratio = Mathf.Clamp(_ArmData.Energy / _ArmData.MaxEnergy, 0, 1);
        SetGlowValue(ratio);
    }
    public void SetGlowValue(float value) {
        if (hasTween) return;

        var duration = Mathf.Abs(value - _h) / _Speed;
        hasTween = true;

        DOTween.To(() => _h, x => _h = x, value, duration).OnComplete(() => { hasTween = false; });
    }
    public void SetGlowValue(float value, float duration)
    {
        if (hasTween) return;

        var from = _h;
        hasTween = true;

        DOTween.To(() => _h, x => _h = x, value, duration).From(_h).SetLoops(2,LoopType.Yoyo).SetEase(_EaseCurve).OnComplete(() => { 
            hasTween = false; 
        });                
    }
    private void Update()
    {
        //ParticleSystem ps = PS; 
        //var em = ps.emission;
        //em.rateOverTime = _h * 200;
        _m1.ForEach(m => m.material.SetFloat("_h", _h));
        _m2.ForEach(m => m.material.SetFloat("_h", (_h - 0.25f) * (4)));
        _m3.ForEach(m => m.material.SetFloat("_h", (_h - 0.50f ) * (4) ));
        _m4.ForEach(m => m.material.SetFloat("_h", (_h - 0.75f ) * (4) ));
        //_m1.material.SetFloat("_h", _h );
        //_m2.material.SetFloat("_h", (_h - 0.25f ) * (4) );
        //_m3.material.SetFloat("_h", (_h - 0.50f ) * (4) );
        //_m4.material.SetFloat("_h", (_h - 0.75f ) * (4) );
    }
}

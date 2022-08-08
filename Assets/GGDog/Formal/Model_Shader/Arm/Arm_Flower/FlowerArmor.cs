using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//[ExecuteInEditMode]
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
    bool hasGainArmorProcess;
    int gainedArmor;
    int maxArmor = 5;
    Sequence dotweeenSep;

    private void Start()
    {
        //Project.EventBus.Subscribe<HelloPico2.InteractableObjects.NeedEnergyEventData>(EnergyUpdate);
    }
    //private void EnergyUpdate(HelloPico2.InteractableObjects.NeedEnergyEventData data) {
    //    if (data.HandType != _ArmData.HandType) return;
    //    var ratio = Mathf.Clamp(_ArmData.Energy / _ArmData.MaxEnergy, 0, 1);
    //    SetGlowValue(ratio);
    //}
    public void SetGlowValue(float value) {
        if (dotweeenSep != null && dotweeenSep.IsPlaying()) return;

        var duration = Mathf.Abs(value - _h) / _Speed;

        SetGlowValue(value, duration, LoopType.Yoyo, 2);
    }
    public void SetGlowValue(float value, float duration, LoopType loopType, int loopAmount)
    {
        if (dotweeenSep != null && dotweeenSep.IsPlaying()) return;

        var from = _h;
        
        if (dotweeenSep != null)
            dotweeenSep.Kill();

        dotweeenSep = DOTween.Sequence();
        dotweeenSep.Append(
             DOTween.To(() => _h, x => _h = x, value, duration).From(_h).SetLoops(loopAmount, loopType).SetEase(_EaseCurve)
        );

        dotweeenSep.Play();               
    }
    float currentHeatLevel;
    public void OverHeat(float step, float speed) {
        if (hasGainArmorProcess) return;

        if (dotweeenSep != null)
            dotweeenSep.Kill();

        currentHeatLevel += step;
        var duration = Mathf.Abs(currentHeatLevel - _h) / speed;
        //var duration = .1f;

        dotweeenSep = DOTween.Sequence();
        dotweeenSep.Append(
             DOTween.To(() => _h, x => _h = x, currentHeatLevel, 0.1f).From(_h).SetEase(_EaseCurve)
        );
        dotweeenSep.Append(
             DOTween.To(() => _h, x => _h = x, 0, Mathf.Abs(currentHeatLevel) / speed).From(currentHeatLevel).SetEase(_EaseCurve)             
        );
        dotweeenSep.Insert(1, DOTween.To(() => currentHeatLevel, x => currentHeatLevel = x, 0, Mathf.Abs(currentHeatLevel) / speed).From(currentHeatLevel));

        dotweeenSep.Play();
    }
    public void GainArmor(float duration, float stayDuration)
    {
        if (gainedArmor >= maxArmor) return;
        
        hasGainArmorProcess = true;

        gainedArmor++;
        Mathf.Clamp(gainedArmor, 0, maxArmor);

        var from = _h;

        if (dotweeenSep != null)
            dotweeenSep.Kill();

        var targetValue = (float)gainedArmor / (float)maxArmor;

        dotweeenSep = DOTween.Sequence();
        dotweeenSep.Append(
            DOTween.To(() => _h, x => _h = x, targetValue, duration).From(0).SetEase(_EaseCurve)
        ).AppendInterval(
            stayDuration
        ).Append(
            DOTween.To(() => _h, x => _h = x, 0, duration).From(targetValue).SetEase(_EaseCurve).OnComplete(() => { hasGainArmorProcess = false; })
        );
        
        dotweeenSep.Play();
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

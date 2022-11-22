using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Game.Project;

public class FollowerShaker : MonoBehaviour
{
    public Follower _Follower;
    public float _Duration = 1;
    public float _Strength = 1;
    ColdDownTimer _ColdDownTimer;

    public Vector3 OriginalOffset { get; private set; }

    private void Awake()
    {
        _ColdDownTimer = new ColdDownTimer(_Duration);
        OriginalOffset = _Follower.m_AdditionalOffset;
    }
    public void StartShake() {
        StartShake(_Strength, _Duration);
    }
    public void StartShake(float Strength, float duration)
    {
        if (!_ColdDownTimer.CanInvoke()) return;

        var Offset = _Follower.m_AdditionalOffset;
        DOTween.Shake(() => Offset, x => Offset = x, duration - 0.05f, Strength).OnUpdate(() => {
            _Follower.m_AdditionalOffset = Offset;
        }).OnComplete(() => {
            DOTween.Shake(() => Offset, x => Offset = x, 0.05f, OriginalOffset).OnUpdate(() =>
            {
                _Follower.m_AdditionalOffset = Offset;
            });
        });

        _ColdDownTimer.ModifyDuring(duration);
        _ColdDownTimer.Reset();
    }
}

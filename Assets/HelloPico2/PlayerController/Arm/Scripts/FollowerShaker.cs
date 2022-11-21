using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowerShaker : MonoBehaviour
{
    public Follower _Follower;
    public float _Duration = 1;
    public float _Strength = 1;

    public void StartShake() {
        StartShake(_Strength, _Duration);
    }
    public void StartShake(float Strength, float duartion) {
        var Offset = _Follower.m_AdditionalOffset;
        DOTween.Shake(() => Offset, x => Offset = x, _Duration, _Strength).OnUpdate(() => {
            _Follower.m_AdditionalOffset = Offset;
        });
    }
}

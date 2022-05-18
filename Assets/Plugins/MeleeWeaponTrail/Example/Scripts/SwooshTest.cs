using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwooshOnOff : MonoBehaviour
{
    /*
    [SerializeField]
    AnimationClip _animation;
    AnimationState _aanimationState;

    [SerializeField]
    int _end = 0;

    float _startN = 0.0f;
    float _endN = 0.0f;

    float _time = 0.0f;
    float _prevTime = 0.0f;
    float _prevAnimTime = 0.0f;
    */
    [SerializeField]
    MeleeWeaponTrail _trail;

    //bool _firstFrame = true;
    public bool Trail = true;

    void Start()
    {
        /*float frames = _animation.frameRate * _animation.length;
        _startN = _start/frames;
        _endN = _end/frames;
        _animationState = GetComponent<Animation>()[_animation.name];*/
        _trail.Emit = true;
    }


	void Update ()
    {
        if (Trail == true)
        {
            _trail.Emit = true;
        }
		else
        {
            _trail.Emit = false;
        }
        /*
        _time += _animationState.normalizedTime - _prevAnimTime;
        if (_time > 1.0f || _firstFrame)
        {
            if (!_firstFrame)
            {
                _time -= 1.0f;
            }
            _firstFrame = false;
        }

        if (_prevTime < _startN && _time >= _startN)
        {
            _trail.Emit = false;
        }

        _prevTime = _time;
        //_prevAnimTime = _animationState.normalizedTime;
        */
	}
}

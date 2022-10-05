using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.InteractableObjects
{
    public class ObjectShaker : MonoBehaviour
    {
        public Transform _ShakeObject;
        public float _Duration = .5f;
        public float _Strength = 1f;
        public int _Vibrato = 10;

        public void StartShaking() {
            _ShakeObject.DOShakePosition(_Duration, _Strength, _Vibrato);
        }
        public void StartShaking(float duration, float strength, int vibrato)
        {
            _ShakeObject.DOShakePosition(duration, strength, vibrato);
        }
        public void StartShaking(float duration, Vector3 strength, int vibrato)
        {
            _ShakeObject.DOShakePosition(duration, strength, vibrato);
        }
    }
}
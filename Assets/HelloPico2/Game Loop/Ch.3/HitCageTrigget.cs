using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitCageTrigget : MonoBehaviour
{
    public GameObject ActiveEffect;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActiveEffect.SetActive(true);
            transform.GetComponent<SphereCollider>().enabled = false;

            OnScaleToZero();
        }
    }
    private void OnScaleToZero()
    {
        transform.DOScale(0, 0.5f)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                ActiveEffect.SetActive(false);
                gameObject.SetActive(false);
            });
    }
}

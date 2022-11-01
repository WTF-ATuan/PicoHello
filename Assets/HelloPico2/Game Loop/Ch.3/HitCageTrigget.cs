using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class HitCageTrigget : MonoBehaviour
{
    public GameObject ActiveEffect;    
    public UltEvent WhenTriggedByPlayer;
    public ICollideFeedback collideFeedback;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractCollider"))
        {
            ActiveEffect.SetActive(true);
            transform.GetComponent<SphereCollider>().enabled = false;

            OnScaleToZero();

            other.TryGetComponent<ICollideFeedback>(out collideFeedback);

            WhenTriggedByPlayer?.Invoke();
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
    public void PlayNormalFeedback() { if(collideFeedback != null) collideFeedback.NormalCollide(); }
    public void PlayCriticalFeedback() { if (collideFeedback != null) collideFeedback.CriticalCollide(); }
}

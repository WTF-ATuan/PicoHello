
using UnityEngine;
using DG.Tweening;

public class EnemyColliderEffect : MonoBehaviour
{
	public GameObject EffectObj;
    float timer = 0;
    float coldTiemr=0.6f;
    bool isTrigget;
    public int counter;
    public bool isTip;
    float sacleTimer=0;
    bool isHit;


    public void Update()
    {
        if (isHit)
        {
            PlayAnim();
        }
         if (isTrigget)
        {
            timer += Time.deltaTime;
            if (timer > coldTiemr)
            {
                EffectObj.SetActive(false);
                isTrigget = false;
                timer = 0;
                
                if (counter < 0)
                {
                    
                    isHit = true;
                }
            }
        }

    }
    void PlayAnim()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.3f);

        Destroy(gameObject, 1);
    }
    private void OnTriggerEnter(Collider other)
    {
        isTrigget = true;
        EffectObj.SetActive(true);
        counter -= 1;
    }

}

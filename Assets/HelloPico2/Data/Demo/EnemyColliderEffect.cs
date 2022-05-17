
using UnityEngine;


public class EnemyColliderEffect : MonoBehaviour
{
	public GameObject EffectObj;
    float timer = 0;
    float coldTiemr=0.6f;
    bool isTrigget;
    public void Update()
    {
        if(isTrigget)
        {
            timer += Time.deltaTime;
            if (timer > coldTiemr)
            {
                EffectObj.SetActive(false);
                isTrigget = false;
                timer = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        isTrigget = true;
        EffectObj.SetActive(true);
    }

}

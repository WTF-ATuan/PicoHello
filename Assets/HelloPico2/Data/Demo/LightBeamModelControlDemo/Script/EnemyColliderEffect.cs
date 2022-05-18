using UnityEngine;
using DG.Tweening;

public class EnemyColliderEffect : MonoBehaviour
{
    public EnemyScore_SO _DemoSO;
    public GameObject EffectObj;
    float timer = 0;
    float coldTiemr=0.6f;
    bool isTrigget;
    public int counter;
    private bool isHit;
    public bool isStep;
    public bool firstType;
    public bool isHitSocre;

    public float speed = 20;
    public Vector3 relativeDirection = Vector3.forward;

    private void Start()
    {
        if (isHitSocre)
        {
            speed = Random.Range(1, speed);
            transform.localScale = new Vector3(Random.Range(0.3f, 1), Random.Range(0.3f, 1), Random.Range(0.3f, 1));
        }
    }
    private void Update()
    {
        if (isHitSocre)
        {
            Vector3 absoluteDirection = transform.rotation * relativeDirection;
            transform.position += absoluteDirection * speed * Time.deltaTime;
        }
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
                    if (isStep &&!firstType)
                    {
                        _DemoSO.getStepHit += 1;
                    }
                    if(isHitSocre)
                    {
                        _DemoSO.getHit += 1;
                    }
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

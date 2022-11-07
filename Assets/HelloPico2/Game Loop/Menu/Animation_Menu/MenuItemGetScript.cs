using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuItemGetScript : MonoBehaviour
{
    public GameObject roundObj;
    Animator roundAnimator;
    

    bool isNotTouch;
    GameObject findPlayer;
    public Animator menuPrefabs;
    public int coldTime;
    public Image filledImage;
    public GameObject rayObj;
    Animator rayObjAnimator;
    float timer;
    public targetScript _targetScript;
    public bool isCh3HitCage;

    [SerializeField] private UnityEvent onGrab;
    [SerializeField] private UnityEvent onTouch;

    private bool _isTouch;
    // Start is called before the first frame update
    void Start()
    {
        
        timer = coldTime;
        if(roundObj) roundAnimator = roundObj.GetComponent<Animator>();
        if(menuPrefabs) menuPrefabs = menuPrefabs.GetComponent<Animator>();
        if(rayObj) rayObjAnimator = rayObj.GetComponent<Animator>();

        findPlayer = GameObject.Find("MenuItemLookPos");

        if(rayObj) filledImage = filledImage.GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(findPlayer) gameObject.transform.LookAt(findPlayer.transform.position);
        

        if (isNotTouch && roundAnimator.speed <1)
        {
            roundAnimator.speed += 0.2f ;
            filledImage.fillAmount -= Time.deltaTime;
            if (roundAnimator.speed == 1)
            {
                isNotTouch = false;
                timer = coldTime;
                filledImage.fillAmount = 0;
            }
        }
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(!_isTouch){
            onTouch?.Invoke();
            _isTouch = true;
        }
        if (other.CompareTag("Player") &&  !isCh3HitCage && roundObj)
        {
            if(roundAnimator.speed > 0)
            {
                roundAnimator.speed -= 0.2f;
            }
            
        }
        
        if (timer > 0 )
        {
            timer -= Time.deltaTime;
            filledImage.fillAmount = (coldTime -timer)/ coldTime;
            if(roundObj) rayObjAnimator.SetBool("isGet", true);
        }
        else
        {
            if(menuPrefabs) menuPrefabs.SetTrigger("isGet");
            _targetScript?.AddItemHeld();
            _targetScript?.LoadTimeLine();
            onGrab?.Invoke();
        }
        

    }
    private void OnTriggerExit(Collider other)
    {
        if (!isCh3HitCage && roundObj)
        {
            rayObjAnimator.SetBool("isGet", false);
            isNotTouch = true;
            _isTouch = false;
        }
    }
}

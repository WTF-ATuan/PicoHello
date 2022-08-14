using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    // Start is called before the first frame update
    void Start()
    {
        timer = coldTime;
        roundAnimator = roundObj.GetComponent<Animator>();
        menuPrefabs = menuPrefabs.GetComponent<Animator>();
        rayObjAnimator = rayObj.GetComponent<Animator>();

        findPlayer = GameObject.Find("MenuItemLookPos");
        
        filledImage = filledImage.GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(findPlayer.transform.position);
        

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
        if (other.CompareTag("Player") && roundAnimator.speed > 0 )
        {
            roundAnimator.speed -= 0.2f;
        }
        
        if (timer > 0 )
        {
            timer -= Time.deltaTime;
            filledImage.fillAmount = (coldTime -timer)/ coldTime;
            rayObjAnimator.SetTrigger("isGet");
        }
        else
        {
            menuPrefabs.SetTrigger("isGet");
            _targetScript.AddItemHeld();
            _targetScript.LoadTimeLine();
        }
        

    }
    private void OnTriggerExit(Collider other)
    {
        isNotTouch = true;
    }
}

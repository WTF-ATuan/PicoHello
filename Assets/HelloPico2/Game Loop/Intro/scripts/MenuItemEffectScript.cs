using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemEffectScript : MonoBehaviour
{
    public float countTime;
    float timer;
    int count= 0;
    bool hideControl;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        hideControl = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hideControl) return;
         timer += Time.deltaTime;
        if (timer > countTime)
        {
            DisableSet();
            timer = 0;
            hideControl = false;
            this.gameObject.SetActive(false);
        }
    }

    void DisableSet()
    {
        ParticleSystem getPsystem = GetComponent<ParticleSystem>();
        var setEm = getPsystem.emission;
        setEm.enabled = false;
    }
}

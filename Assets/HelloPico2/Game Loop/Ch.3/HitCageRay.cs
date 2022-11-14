using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitCageRay : MonoBehaviour
{
    public Image filledImage;
    public int coldTime;
    float timer;
    bool isHit;
    // Start is called before the first frame update
    void Start()
    {
        if (filledImage) filledImage = filledImage.GetComponent<Image>();
        timer = coldTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit)
        {
            timer -= Time.deltaTime;
            filledImage.fillAmount = (coldTime - timer) / coldTime;
            if (filledImage.fillAmount == 1)
            {
                isHit = false;
                timer = coldTime;
                Debug.Log("Run");
            }
        }
    }
    public void HitCh3CageImage()
    {
        isHit = true;
    }

}

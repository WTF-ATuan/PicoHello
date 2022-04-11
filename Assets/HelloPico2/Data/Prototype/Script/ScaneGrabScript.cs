using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaneGrabScript : MonoBehaviour
{
    GameObject portal;
    public int scaneType;
    public float scaleSize=0.1f;
    public AudioClip hitAudio;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        portal = GameObject.Find("portalDoor");
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (scaneType == 0)
            {
                portal.transform.localScale = new Vector3(portal.transform.localScale.x + scaleSize, portal.transform.localScale.y, portal.transform.localScale.z + scaleSize);
            }
            else
            {
                if(portal.transform.localScale.x > 0.3f)
                {
                    portal.transform.localScale = new Vector3(portal.transform.localScale.x - scaleSize, portal.transform.localScale.y, portal.transform.localScale.z - scaleSize);
                }
                
            }
            source.PlayOneShot(hitAudio, 1);
            Destroy(gameObject, 0.5f);
        }
    }
}

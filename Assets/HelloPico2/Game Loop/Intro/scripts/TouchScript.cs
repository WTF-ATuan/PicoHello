using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScript : MonoBehaviour
{
    float scaleSize;
    int touchCounter;
    public Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        scaleSize = 0.5f;
        _animator = _animator.GetComponent<Animator>();
    }

    // Update is called once per frame

    public void OnTriggerEnter(Collider other)
    {
        _animator.SetTrigger("isHit");

        other.gameObject.transform.localScale -= new Vector3(scaleSize, scaleSize, scaleSize);

        

        if (other.gameObject.transform.localScale.x < 0.1f)
        {
            _animator.SetTrigger("isOpen");
            Destroy(other.gameObject);
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class targetEventScript : MonoBehaviour
{
    public Animator _animator;
    public GameObject _particle;
    GameObject insObj;
    public bool isAnim;

    float coldTime;
    float timer;
    void Start()
    {
        if (isAnim)
        {
            _animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        }
        
    }

    public void handSelectEntered()
    {
        if (isAnim)
        {
            _animator.SetBool("isGet", true);
            _animator.Play(1);
        }

        createParticle();
        Destroy(gameObject, 5);
    }
    // Update is called once per frame
  
    private void OnTriggerEnter(Collider other)
    {

        InvokeRepeating("createParticle",1, 5);
        if (isAnim)
        {
            _animator.SetBool("isGet", true);
            _animator.Play(1);
        }
        // Destroy(gameObject,5);

    }

    public void createParticle()
    {
        insObj = Instantiate(_particle, transform.position, transform.rotation, this.gameObject.transform);
        insObj.transform.localPosition = Vector3.zero;
    }
}

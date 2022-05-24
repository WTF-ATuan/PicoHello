using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayTargetEventScript : MonoBehaviour
{
    public TargetItem_SO _targetItem;
    public float coldTime;
    float timer;
    public Animator _animator;
    public GameObject _particle;
    GameObject insObj;
    bool isGet=false;

    void Start()
    {
        timer = coldTime;
        _animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        
    }

    public void handSelectEntered()
    {
        if (isGet == false)
        {
            createParticle();
        }
        isGet = true;
        _animator.SetBool("isGet", true);
        if (timer == coldTime)
        {
            _targetItem.targetItemHeld += 1;
            timer += Time.deltaTime;
        }
        
        //_animator.Play(1);


    }
    // Update is called once per frame
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InvokeRepeating("createParticle",1, 5);
            _animator.SetBool("isGet", true);
            _animator.Play(1);
            Destroy(gameObject,5);
        }
    }
    public void LoadSceneFun()
    {
        SceneManager.LoadScene(sceneNum);
    }
    */
    public void createParticle()
    {
        insObj = Instantiate(_particle, transform.position, transform.rotation, this.gameObject.transform);
        insObj.transform.localPosition = Vector3.zero;
    }
}

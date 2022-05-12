using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShootScript : MonoBehaviour
{
    //shootBall
    public float speed;
    public GameObject Hitting_Effect;
    TrailRenderer _TrailRenderer;
    
    bool isCheck;
    private void Awake()
    {
        _TrailRenderer = transform.GetChild(0).GetComponent<TrailRenderer>();
    }

    GameObject Player_Hand_Pos;
    void OnEnable()
    {
        _TrailRenderer.enabled = true;
    }
    private void Start()
    {
        GetComponent<SphereCollider>().radius = 0.3f;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target")&&transform.tag=="ballAttack")
        {
            speed = 0;
            Hitting_Effect.SetActive(false);
            Hitting_Effect.SetActive(true);
            Invoke("hideObj", 1);
        }
    }
    public void changeTag()
    {
        transform.tag = "ballAttack";
    }
    void hideObj()
    {
        transform.tag = "ball";
        gameObject.SetActive(false);
    }

}

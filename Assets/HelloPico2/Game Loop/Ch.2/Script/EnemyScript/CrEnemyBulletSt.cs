using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum protoEnemyStates {GUARD,ATTACK, PATROL,DEAD }
public class CrEnemyBulletSt : MonoBehaviour
{
    public int enemyType; //0 initiative 1,passive
    public protoEnemyStates enemyStates;
    public Animator _animator;
    public GameObject[] bulletList;
    public Vector2 AttackTimeRange;
    public float patrolRange;
    private Vector3 wayPoint; //Patrol
    float timer = 0;
    public int hitDead = 3;
    GameObject parentName;
    GameObject parentObj;
    // Start is called before the first frame update
    void Start()
    {
        //playerLocal = GameObject.Find("Main Camera");
        parentObj = gameObject.transform.parent.gameObject;
        parentName = GameObject.Find("EnemyPool");
        if (enemyType == 0)
        {
            parentObj.GetComponent<Transform>().LookAt(Vector3.zero);
            Instantiate(bulletList[0], transform.position, Quaternion.identity, parentName.transform);
        }
        _animator = _animator.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        SwitchType();
        SwitchStates();
    }
    void SwitchStates()
    {
        switch (enemyStates)
        {
            case protoEnemyStates.GUARD:
                break;

            case protoEnemyStates.ATTACK:
                _animator.SetBool("isAttack", true);
                createBullet();
                break;

            case protoEnemyStates.PATROL:
                if(wayPoint ==transform.position)
                {
                    enemyStates = protoEnemyStates.GUARD;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, wayPoint, 0.2f);
                    transform.Rotate(Vector3.forward * 0.5f);
                }
                break;

            case protoEnemyStates.DEAD:
                _animator.SetBool("isDead", true);
                Destroy(gameObject, 1);//Destroy(gameObject.transform.parent.gameObject, 1);
                break;
        }
    }
    void SwitchType()
    {
        if (hitDead == 0)
        {
            enemyStates = protoEnemyStates.DEAD;
        }
        if (enemyType == 0 && hitDead!=0)//changeTo Attack
        {
            float AttackTime = Random.Range(AttackTimeRange.x, AttackTimeRange.y);

            timer += Time.deltaTime;
            if (timer > AttackTime)
            {
                enemyStates = protoEnemyStates.ATTACK;
            }
        }
        if(enemyType == 1 && hitDead == 0)
        {
            enemyStates = protoEnemyStates.DEAD;
        }

    }
    void createBullet()
    {
        if (hitDead != 0)
        {
            int randBullet = Random.Range(0, bulletList.Length);
            Instantiate(bulletList[randBullet], transform.position, Quaternion.identity, parentName.transform);
            _animator.SetBool("isAttack", false) ;
            GetNewPoint();
            enemyStates = protoEnemyStates.PATROL;
        }
        timer = 0;
    }
    void GetNewPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        Vector3 ramdomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);
        wayPoint = ramdomPoint;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ball")
        {
            hitDead -= 1;
        }
    }
}


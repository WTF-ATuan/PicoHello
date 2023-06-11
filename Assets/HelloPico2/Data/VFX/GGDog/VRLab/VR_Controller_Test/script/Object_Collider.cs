using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Object_Kind))]

public class Object_Collider : MonoBehaviour
{

    public GameObject hit;

    Object_Kind _Object_Kind;

    private void Awake()
    {
        _Object_Kind = GetComponent<Object_Kind>();
    }


    bool _Destroy=false;

    private void Update()
    {
        if(_Destroy)
        {
            Destroy(gameObject);
        }
    }

    Transform _col_Transform;
    Object_Collider _col_Collider;
    Object_Kind _col_Kind;

    void OnCollisionEnter(Collision col)
    {
        _col_Transform = col.transform;

        _col_Collider = col.gameObject.GetComponent<Object_Collider>();

        _col_Kind = col.gameObject.GetComponent<Object_Kind>();

        //就只做玩家對敵人的單方判斷就好，不然敵人也做一次重複判斷就多餘了
        if (_Object_Kind._o ==0 && _col_Kind._o == 1)
        {
                //我方的
                switch (_Object_Kind._k)
                {
                    case 0: //我出剪刀
                    switch (_col_Kind._k)
                        {
                            //剪-剪
                            case 0: discharge(); break;
                            //剪-布
                            case 1: destroy(); break;
                            //剪-石
                            case 2: destroySelf(); break;
                        }
                        break;

                    case 1: //我出布
                    switch (_col_Kind._k)
                        {
                            //布-剪
                            case 0: destroySelf(); break;
                            //布-布
                            case 1:  discharge(); break;
                            //布-石
                            case 2:  destroy(); break;
                        }
                        break;

                    case 2: //我出石
                    switch (_col_Kind._k)
                        {
                            //石-剪
                            case 0: destroy(); break;
                            //石-布
                            case 1: destroySelf(); break;
                            //石-石
                            case 2: discharge(); break;
                        }
                        break;
                }
        }
    }

    //平手-互相摧毀
    void discharge()
    {
        Instantiate(hit, transform.position, transform.rotation);
        Instantiate(_col_Collider.hit, _col_Transform.position, _col_Transform.rotation);

        _col_Collider._Destroy = true;
        _Destroy = true;
    }

    //贏-摧毀敵人的
    void destroy()
    {
        Instantiate(_col_Collider.hit, _col_Transform.position, _col_Transform.rotation);
        _col_Collider._Destroy = true;
    }

    //輸-摧毀自己的
    void destroySelf()
    {
        Instantiate(hit, transform.position, transform.rotation);
        _Destroy = true;
    }


}

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

        //�N�u�����a��ĤH�����P�_�N�n�A���M�ĤH�]���@�����ƧP�_�N�h�l�F
        if (_Object_Kind._o ==0 && _col_Kind._o == 1)
        {
                //�ڤ誺
                switch (_Object_Kind._k)
                {
                    case 0: //�ڥX�ŤM
                    switch (_col_Kind._k)
                        {
                            //��-��
                            case 0: discharge(); break;
                            //��-��
                            case 1: destroy(); break;
                            //��-��
                            case 2: destroySelf(); break;
                        }
                        break;

                    case 1: //�ڥX��
                    switch (_col_Kind._k)
                        {
                            //��-��
                            case 0: destroySelf(); break;
                            //��-��
                            case 1:  discharge(); break;
                            //��-��
                            case 2:  destroy(); break;
                        }
                        break;

                    case 2: //�ڥX��
                    switch (_col_Kind._k)
                        {
                            //��-��
                            case 0: destroy(); break;
                            //��-��
                            case 1: destroySelf(); break;
                            //��-��
                            case 2: discharge(); break;
                        }
                        break;
                }
        }
    }

    //����-���ۺR��
    void discharge()
    {
        Instantiate(hit, transform.position, transform.rotation);
        Instantiate(_col_Collider.hit, _col_Transform.position, _col_Transform.rotation);

        _col_Collider._Destroy = true;
        _Destroy = true;
    }

    //Ĺ-�R���ĤH��
    void destroy()
    {
        Instantiate(_col_Collider.hit, _col_Transform.position, _col_Transform.rotation);
        _col_Collider._Destroy = true;
    }

    //��-�R���ۤv��
    void destroySelf()
    {
        Instantiate(hit, transform.position, transform.rotation);
        _Destroy = true;
    }


}

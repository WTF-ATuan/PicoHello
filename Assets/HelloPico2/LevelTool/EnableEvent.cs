using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using UnityEngine.Serialization;

public class EnableEvent : MonoBehaviour
{
    public UltEvent WhenEnable;
    public UltEvent WhenEnableDelay;
    public float m_DelayTime = 1;
    public bool invokeByOrder;
    public void OnEnable()
    {
        if(!invokeByOrder){
            WhenEnable.Invoke();
            StartCoroutine(Delayer());
        }
        else{
            StartCoroutine(Invoker());
        }
    }
    private IEnumerator Delayer() {
        yield return new WaitForSeconds(m_DelayTime);
        WhenEnableDelay.Invoke();
    }

    private IEnumerator Invoker(){
        foreach(var call in WhenEnable.PersistentCallsList){
            yield return null;
            call.Invoke();
        }
    }
}

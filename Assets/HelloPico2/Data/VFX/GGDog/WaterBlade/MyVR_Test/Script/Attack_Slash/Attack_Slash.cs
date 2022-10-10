using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Slash : MonoBehaviour
{

    public GameObject GO;
    public GameObject HandController;
    public float Sensitive=0.05f;
    
    Vector3 currentDir;
    Vector3 deltaDir;
    Vector3 lastDir = new Vector3(0, 0, 0);
    
    bool IsGo = false;

    void Update()
    {
        currentDir = HandController.transform.position;
        deltaDir = currentDir - lastDir;
        lastDir = currentDir;
        
        if (Vector3.Magnitude(deltaDir) >= Sensitive && !IsGo)
        {
            Instantiate(GO, HandController.transform.position, HandController.transform.rotation);
            IsGo = true;
        }
        if (Vector3.Magnitude(deltaDir) < 0.03F && IsGo)
        {
            IsGo = false;
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]

public class JointMaxDistance : MonoBehaviour {


    public float Angle = 30;
    public float rate = 100;

    public GameObject Parent;

    public float MaxDistance = 0.1f;
    
    Vector3 currentDir;
    static public Vector3 deltaDir;
    Vector3 lastDir=new Vector3(0,0,0);
    
    void Update ()
    {

        currentDir = transform.position;
        deltaDir = currentDir -lastDir ;
        lastDir = currentDir;
        
       // deltaDir = Vector3.Lerp(deltaDir ,ChaseObject.HandZDir*10, Vector3.Dot(deltaDir, ChaseObject.HandZDir));
        
        deltaDir = Vector3.ClampMagnitude(deltaDir *1.1f , MaxDistance);
        deltaDir -= Vector3.ClampMagnitude(deltaDir , -MaxDistance/(rate*1.5f));
        
        //使整體往速度方向變大，做出"甩出"的感覺
       // transform.Translate(deltaDir*(1+Vector3.Dot(deltaDir, ChaseObject.HandZDir)) / 1.3f);
        transform.Translate(deltaDir  / 1.3f);

        //鏈結:限制兩物體之間的距離
        if (Vector3.Distance(transform.position, Parent.transform.position) > MaxDistance)
        {
            //取得兩物體之間的單位向量再乘以最大限制長度
            Vector3 pos = (transform.position - Parent.transform.position).normalized * MaxDistance;
            
            //在連結物件位置加上此偏移量
            transform.position = pos + Parent.transform.position;
        }

        
        if (Parent.GetComponent<JointMaxDistance>())
        {
            Vector3 P2PP = Parent.transform.position - Parent.GetComponent<JointMaxDistance>().Parent.transform.position;
        
            float angle = angle_360( transform.position - Parent.transform.position , P2PP );

            if(angle> Angle)
            {
               Vector3 pos = (ChaseObject.HandZDir).normalized * MaxDistance;
            
               transform.position = Vector3.Lerp(transform.position, pos + Parent.transform.position, Vector3.Magnitude(deltaDir) * (angle - Angle) / angle);
            }
        }
        
    }


    
    float angle_360(Vector3 from_, Vector3 to_)
    {
        Vector3 v3 = Vector3.Cross(from_, to_);
        if (v3.z > 0)
            return Vector3.Angle(from_, to_);
        else
            return 180 - Vector3.Angle(from_, to_);
    }
}

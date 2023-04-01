using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Moving_enemy : MonoBehaviour
{
    public float speed;

    public Vector3 destination = new Vector3(0, -0.15f, -0.15f);

    void Update()
    {
         //向鏡頭後方飛
           transform.position = Vector3.Lerp(transform.position, destination, speed * 0.01f);

         //角度瞄準到鏡頭處
           Vector3 axis = Vector3.Cross(transform.rotation * Vector3.forward, transform.position - new Vector3(0, 0, -0.15f)).normalized;
           float angle = Vector3.Angle(transform.rotation * Vector3.forward, transform.position - new Vector3(0, 0, -0.15f));
           transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, axis) * transform.rotation, 0.75f);

         //飛過頭了就刪除
           if (transform.position.z <= -3f) { Destroy(gameObject); }


           transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, 0.5f);


    }
}

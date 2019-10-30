using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float speed = 50;
    Vector3 target;

    void Start()
    {
        target = transform.position;
    }
    public void Move(Transform pointZero)
    {
        target = pointZero.position;
    }

    void Update()
    {
        if(target != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target, speed * Time.deltaTime);
        }
    }
}

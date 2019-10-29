using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHearth : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("Pump");
    }

    IEnumerator Pump()
    {
        yield return new WaitForSecondsRealtime(3);
        animator.Play("Pump");
        StartCoroutine("Pump");
    }
}

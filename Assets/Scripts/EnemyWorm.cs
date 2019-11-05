using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWorm : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

}

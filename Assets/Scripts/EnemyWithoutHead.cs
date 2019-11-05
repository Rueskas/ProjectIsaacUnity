using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithoutHead : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private GameObject player;
    [SerializeField] private float minTimeWaiting = 1;
    [SerializeField] private float maxTimeWaiting = 3;
    [SerializeField] private int live = 100;
    [SerializeField] private float vectorDistanceIncreaser = 3f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>().gameObject;
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        yield return new WaitForSecondsRealtime(Random.Range(minTimeWaiting, maxTimeWaiting));
        Vector2 vectorDirection = player.transform.position - transform.position;
        rb2D.AddForce(vectorDirection * vectorDistanceIncreaser, ForceMode2D.Impulse);
        float angle = AngleBetween(Vector2.zero, vectorDirection);
        if( (angle < 0 && angle > -45) || (angle > 0 && angle < 45) )
        {
            animator.Play("RunLeftRight");
            spriteRenderer.flipX = false;
        }
        else if ( angle < -135  || angle > 135)
        {
            animator.Play("RunLeftRight");
            spriteRenderer.flipX = true;
        }
        else
        {
            animator.Play("RunUpDown");
        }
        StartCoroutine("Move");
    }

    void Update()
    {
        
        if(rb2D.velocity != Vector2.zero)
        {
            animator.SetBool("Moving", false);
        }
        else
        {
            animator.SetBool("Moving", true);
        }

    }

    IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Tear")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= collision.gameObject.GetComponent<Tear>().GetDamage();
            if (live <= 0)
            {
                transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
                Destroy(this.gameObject);
            }
        }
        if (collision.gameObject.tag == "ItemPasiveDamage")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= FindObjectOfType<GameController>().GetLevel() * 15;
            if (live <= 0)
            {
                transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
                Destroy(this.gameObject);
            }
        }
    }

    private float AngleBetween(Vector2 v1, Vector2 v2)
    {
        Vector2 diference = v2 - v1;
        float sign = (v2.y < v1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }
}

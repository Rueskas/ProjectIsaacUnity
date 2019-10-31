using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniPop : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private float minTimeWaiting = 0.5f;
    [SerializeField] private float maxTimeWaiting = 1.5f;
    [SerializeField] private int live = 40;
    [SerializeField] private float vectorDistanceIncreaser = 1.5f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isInvoked = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        yield return new WaitForSecondsRealtime(Random.Range(minTimeWaiting, maxTimeWaiting));
        Vector2 vectorDirection = Random.insideUnitCircle.normalized;
        animator.Play("Attacking");
        rb2D.AddForce(vectorDirection * vectorDistanceIncreaser, ForceMode2D.Impulse);
        float angle = AngleBetween(Vector2.zero, vectorDirection);
        if ((angle < 0 && angle > -90) || (angle > 0 && angle < 90))
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        StartCoroutine("Move");
    }

    void Update()
    {
        if (live <= 0)
        {
            if (!isInvoked)
            {
                transform.parent.GetComponent<Room>()
                    .SendMessage("EnemyDeath");
            }
            Destroy(this.gameObject);
        }
        if (rb2D.velocity == Vector2.zero)
        {
            animator.SetBool("Attacking", true);
        }
        else
        {
            animator.SetBool("Attacking", false);
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
        if (collision.tag == "Tear")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= collision.gameObject.GetComponent<Tear>().GetDamage();
        }
    }

    private float AngleBetween(Vector2 v1, Vector2 v2)
    {
        Vector2 diference = v2 - v1;
        float sign = (v2.y < v1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }

    public void SetIsInvoked(bool isInvoked)
    {
        this.isInvoked = isInvoked;
    }
}

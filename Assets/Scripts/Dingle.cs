using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dingle : MonoBehaviour
{
    private float baseLive = 1000;
    private float liveBonus = 200;
    private float totalLive;
    private float currentLive;
    private GameObject imageCurrentLive;
    private float offsetSubstract = 0.048f;
    private float offsetPercent = 0.02666f;
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float timeWaiting;
    private int phase = 1;
    [SerializeField] private GameObject minipop;
    [SerializeField] private float minTimeWaiting = 1f;
    [SerializeField] private float maxTimeWaiting = 3f;
    [SerializeField] private float vectorDistanceIncreaser = 3f;
    private int countAttacks = 0;

    void Start()
    {
        int level = FindObjectOfType<GameController>().GetLevel();
        currentLive = totalLive = baseLive + (liveBonus * level);
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("FindImageBar", 1f);
        StartCoroutine("Attack");
    }

    public void FindImageBar()
    {
        imageCurrentLive =
            transform.parent.Find("CurrentLive").gameObject;
        transform.parent = null;
    }

    void Update()
    {
        if(currentLive == totalLive / 2)
        {
            phase = 2;
        }
        if(countAttacks == 3)
        {
            countAttacks = 0;
            animator.Play("FinishedAttack");
            StartCoroutine("Attack");
        }
    }

    IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;

    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(
            Random.Range(minTimeWaiting, maxTimeWaiting));
        if(phase == 1)
        {
            animator.Play("PreparingAttack");
        }
    }

    private void AttackPhase1()
    {
        GameObject player = FindObjectOfType<Player>().gameObject;
        Vector2 vectorDirection = player.transform.position - transform.position;
        rb2D.AddForce(vectorDirection * vectorDistanceIncreaser, ForceMode2D.Impulse);
        float angle = AngleBetween(Vector2.zero, vectorDirection);
        if ((angle < 0 && angle > -45) || (angle > 0 && angle < 45))
        {
            spriteRenderer.flipX = false;
        }
        else if (angle < -135 || angle > 135)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void IncreaseCountAttack()
    {
        countAttacks++;
    }

    public void Destroy()
    {
        transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
        Destroy(this.gameObject);
    }

    public void DecreaseLiveBar(float damage)
    {
        float newScaleX = (currentLive - damage) / totalLive;
        float substract =
            ((damage / totalLive) * offsetSubstract) / offsetPercent;
        currentLive -= damage;
        imageCurrentLive.transform.localScale =
            new Vector3(newScaleX,
                imageCurrentLive.transform.localScale.y);
        imageCurrentLive.transform.position =
            new Vector3(imageCurrentLive.transform.position.x - substract,
                imageCurrentLive.transform.position.y,
                imageCurrentLive.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Tear")
        {
            StartCoroutine("ChangeColorDamaged");
            float damage =
                    collision.gameObject.GetComponent<Tear>().GetDamage();
            DecreaseLiveBar(damage);
            if (currentLive <= 0)
            {
                imageCurrentLive.SetActive(false);
                animator.Play("Death");
            }
        }
        if (collision.gameObject.tag == "ItemPasiveDamage")
        {
            StartCoroutine("ChangeColorDamaged");
            float damage = 
                FindObjectOfType<GameController>().GetLevel() *
                     GameController.damagePasiveItems;
            DecreaseLiveBar(damage);
            if (currentLive <= 0)
            {
                imageCurrentLive.SetActive(false);
                animator.Play("Death");
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

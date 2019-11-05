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

    void Start()
    {
        int level = FindObjectOfType<GameController>().GetLevel();
        currentLive = totalLive = baseLive + (liveBonus * level);
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("FindImageBar", 1f);
    }

    public void FindImageBar()
    {
        imageCurrentLive =
            transform.parent.Find("CurrentLive").gameObject;
    }

    void Update()
    {
        
    }

    IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;

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
            float damage = FindObjectOfType<GameController>().GetLevel() * 15;
            DecreaseLiveBar(damage);
            if (currentLive <= 0)
            {
                imageCurrentLive.SetActive(false);
                animator.Play("Death");
            }
        }
    }
}

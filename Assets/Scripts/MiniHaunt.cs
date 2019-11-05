using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniHaunt : MonoBehaviour
{

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float speed = 2f;
    private bool isActive = false;
    [SerializeField] private int live = 200;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isActive)
        {
            Transform player = FindObjectOfType<Player>().transform;
            transform.position = 
                Vector2.MoveTowards(
                    transform.position, player.position, speed * Time.deltaTime);
        }
    }

    public void Active()
    {
        animator.Play("Attacking");
        spriteRenderer.color = Color.white;
        isActive = true;
        transform.parent = null;
    }

    IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.tag == "Tear")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= collision.gameObject.GetComponent<Tear>().GetDamage();
            if (live <= 0)
            {
                FindObjectOfType<TheHaunt>().DiedMiniHaunt();
                Destroy(this.gameObject);
            }
        }
        if (isActive && collision.gameObject.tag == "ItemPasiveDamage")
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

}

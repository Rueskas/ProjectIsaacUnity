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


    private void Damaged(int damage)
    {
        StartCoroutine("ChangeColorDamaged");
        live -= damage;
        if (live <= 0)
        {
            FindObjectOfType<TheHaunt>().DiedMiniHaunt();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.tag == "Tear")
        {
            int damage = collision.GetComponent<Tear>().GetDamage();
            Damaged(damage);
        }
        if (isActive && collision.gameObject.tag == "ItemPasiveDamage")
        {

            int damage = FindObjectOfType<GameController>().GetLevel()
                * GameController.damagePasiveItems;
            Damaged(damage);
        }
    }

}

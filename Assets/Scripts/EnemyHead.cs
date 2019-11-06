using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    [SerializeField] private float minTimeWaiting = 1.5f;
    [SerializeField] private float maxTimeWaiting = 2.5f;
    [SerializeField] private int live = 80;
    [SerializeField] private float vectorDistanceIncreaser = 1.5f;
    [SerializeField] private GameObject tear;
    [SerializeField] private float shotSpeed = 50f;
    [SerializeField] private float timeShot = 1;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("PrepareShot", Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    private void PrepareShot()
    {
        animator.Play("Shot");
        Invoke("PrepareShot", Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    public void InstantiateShot()
    {
        Transform player = FindObjectOfType<Player>().transform;
        Vector3 direction = player.position - transform.position;
        GameObject tearInstantiated =
            Instantiate(
                tear,
                transform.position + direction.normalized/2, 
                Quaternion.identity);
        tearInstantiated.GetComponent<Rigidbody2D>()
            .AddForce(direction * shotSpeed);
        tearInstantiated.GetComponent<EnemyTear>()
            .Invoke("StartAnim", timeShot);
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
        if (collision.gameObject.tag == "ItemPasiveDamage")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= FindObjectOfType<GameController>().GetLevel() *
                 GameController.damagePasiveItems;
        }

        if (live <= 0)
        {
            transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
            Destroy(this.gameObject);
        }
    }
}

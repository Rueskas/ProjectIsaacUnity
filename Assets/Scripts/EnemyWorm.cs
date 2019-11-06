using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWorm : MonoBehaviour
{
    [SerializeField] private float minTimeWaitingShot = 1f;
    [SerializeField] private float maxTimeWaitingShot = 3f;
    [SerializeField] private float minTimeWaiting = 1f;
    [SerializeField] private float maxTimeWaiting = 3f;
    [SerializeField] private int live = 80;
    [SerializeField] private float vectorDistanceIncreaser = 1.5f;
    [SerializeField] private GameObject tear;
    [SerializeField] private float shotSpeed = 50f;
    [SerializeField] private float timeShot = 1;
    [SerializeField] private float speed = 1f;
    private Bounds bounds;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("Hidden");
        bounds = 
            transform.parent.GetChild(0).GetComponent<Collider2D>().bounds;
        Invoke("Appear",
                Random.Range(minTimeWaiting, maxTimeWaiting));
    }


    IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;
    }

    private void PrepareShot()
    {
        animator.Play("Shot");
        Invoke("PrepareShot", Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    public void InstantiateShot()
    {
        Transform player = FindObjectOfType<Player>().transform;
        Vector2 direction = player.position - transform.position;
        GameObject tearInstantiated =
            Instantiate(
                tear,
                transform.position,
                Quaternion.identity);
        tearInstantiated.GetComponent<Rigidbody2D>()
            .AddForce(direction * shotSpeed);
        tearInstantiated.GetComponent<EnemyTear>()
            .Invoke("StartAnim", timeShot);
    }

    private void Appear()
    {
        animator.Play("Appears");
        GetComponent<Collider2D>().enabled = true;
        Invoke("Shot",
                Random.Range(minTimeWaitingShot, maxTimeWaitingShot));
    }

    private void Shot()
    {
        Transform player = FindObjectOfType<Player>().transform;
        Vector3 direction = player.position - transform.position;
        GameObject tearInstantiated =
            Instantiate(
                tear,
                transform.position + direction.normalized / 2,
                Quaternion.identity);
        tearInstantiated.GetComponent<Rigidbody2D>()
            .AddForce(direction * shotSpeed);
        tearInstantiated.GetComponent<EnemyTear>()
            .Invoke("StartAnim", timeShot);
        Invoke("Dissapear",
            Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    private void Dissapear()
    {
        animator.Play("Dissapear");
        GetComponent<Collider2D>().enabled = false;
        Invoke("Appear",
                Random.Range(minTimeWaiting, maxTimeWaiting));
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

    public Vector2 RandomPointInBounds()
    {
        Vector2 randomPosition = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
        return randomPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithoutHead : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    [SerializeField] private GameObject player;
    [SerializeField] private float minTimeWaiting = 1;
    [SerializeField] private float maxTimeWaiting = 3;
    [SerializeField] private int live = 100;
    [SerializeField] private float vectorDistanceIncreaser = 2.5f;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().gameObject;
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        yield return new WaitForSecondsRealtime(Random.Range(minTimeWaiting, maxTimeWaiting));
        Vector2 vectorDirection = player.transform.position - transform.position;
        rigidbody2D.AddForce(vectorDirection* vectorDistanceIncreaser, ForceMode2D.Impulse);
        StartCoroutine("Move");
    }

    void Update()
    {
        if(live <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Tear")
        {
            live -= collision.gameObject.GetComponent<Tear>().GetDamage();
        }
    }
}

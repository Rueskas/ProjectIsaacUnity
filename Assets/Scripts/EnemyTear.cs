using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTear : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.lossyScale.Set(1, 1, 1);
        if (collision.tag != "Enemy")
        {
            StartAnim();
        }
    }

    public void StartAnim()
    {
        GetComponent<Animator>().Play("Touch");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}

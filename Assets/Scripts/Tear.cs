using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tear : MonoBehaviour
{
    [SerializeField] private int damage = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.lossyScale.Set(1, 1, 1);
        if(collision.tag != "Player")
        {
            StartAnim();
        }
    }

    public int GetDamage()
    {
        return damage;
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

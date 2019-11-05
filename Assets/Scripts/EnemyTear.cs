using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTear : MonoBehaviour
{
    [SerializeField] private AudioClip touchedClip;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.lossyScale.Set(1, 1, 1);
        if (collision.gameObject.tag != "Enemy" &&
                collision.gameObject.tag != "EnemyTear")
        {
            StartAnim();
        }
    }

    public void StartAnim()
    {
        GetComponent<Animator>().Play("Touch");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        AudioSource.PlayClipAtPoint(touchedClip, transform.position);

        Invoke("Destroy", 0.5f);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}

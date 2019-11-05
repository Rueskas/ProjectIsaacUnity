using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tear : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private AudioClip throwClip;
    [SerializeField] private AudioClip touchedClip;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(throwClip, transform.position);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.lossyScale.Set(1, 1, 1);
        if(collision.tag != "Tear"
            && collision.tag != "ItemPasiveDamage")
        {
            StartAnim();
        }
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public Vector2 GetScale()
    {
        return transform.localScale;
    }

    public void AddScale(Vector3 addScale)
    {
        transform.localScale += addScale;
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
